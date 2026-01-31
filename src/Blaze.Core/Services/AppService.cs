using System.Diagnostics;
using Blaze.Core.Data;
using Blaze.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blaze.Core.Services;

/// <summary>
/// Implementation of the application service
/// </summary>
public class AppService : IAppService
{
    private readonly BlazeDbContext _context;
    private readonly ILogger<AppService> _logger;
    private readonly Dictionary<string, Process> _runningProcesses = new();

    public AppService(BlazeDbContext context, ILogger<AppService> logger)
    {
        _context = context;
        _logger = logger;
    }

    #region Store/Catalog Operations

    public async Task<IEnumerable<Application>> GetAllAppsAsync()
    {
        return await _context.Applications
            .Include(a => a.Category)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetFeaturedAppsAsync()
    {
        // Return top-rated apps with high download counts
        return await _context.Applications
            .Where(a => a.Rating >= 4.0 && a.DownloadCount > 1000)
            .OrderByDescending(a => a.Rating)
            .ThenByDescending(a => a.DownloadCount)
            .Take(10)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetPopularAppsAsync(int count = 10)
    {
        return await _context.Applications
            .OrderByDescending(a => a.DownloadCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetNewReleasesAsync(int count = 10)
    {
        var thirtyDaysAgo = DateTime.Now.AddDays(-30);
        return await _context.Applications
            .Where(a => a.ReleaseDate >= thirtyDaysAgo)
            .OrderByDescending(a => a.ReleaseDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetAppsByCategoryAsync(string categoryId)
    {
        return await _context.Applications
            .Where(a => a.CategoryId == categoryId)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> SearchAppsAsync(string query)
    {
        var lowerQuery = query.ToLowerInvariant();
        return await _context.Applications
            .Where(a => a.Name.ToLower().Contains(lowerQuery) ||
                        a.Description.ToLower().Contains(lowerQuery) ||
                        a.Developer.ToLower().Contains(lowerQuery) ||
                        a.Tags.Any(t => t.ToLower().Contains(lowerQuery)))
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<Application?> GetAppByIdAsync(string appId)
    {
        return await _context.Applications
            .Include(a => a.Category)
            .FirstOrDefaultAsync(a => a.Id == appId);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    #endregion

    #region Library Operations

    public async Task<IEnumerable<InstalledApp>> GetInstalledAppsAsync()
    {
        return await _context.InstalledApps
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    public async Task<InstalledApp?> GetInstalledAppAsync(string appId)
    {
        return await _context.InstalledApps
            .FirstOrDefaultAsync(a => a.ApplicationId == appId);
    }

    public async Task<bool> IsAppInstalledAsync(string appId)
    {
        return await _context.InstalledApps
            .AnyAsync(a => a.ApplicationId == appId);
    }

    #endregion

    #region App Management

    public async Task<bool> InstallAppAsync(Application app, string installPath)
    {
        try
        {
            // Create installed app record
            var installedApp = new InstalledApp
            {
                ApplicationId = app.Id,
                Name = app.Name,
                InstalledVersion = app.Version,
                InstallPath = installPath,
                ExecutablePath = Path.Combine(installPath, app.ExecutablePath),
                InstalledSizeBytes = app.SizeInBytes,
                InstalledAt = DateTime.Now,
                IconPath = Path.Combine(installPath, "icon.ico")
            };

            await _context.InstalledApps.AddAsync(installedApp);

            // Update application installed status
            var appEntity = await _context.Applications.FindAsync(app.Id);
            if (appEntity != null)
            {
                appEntity.IsInstalled = true;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("App {AppName} installed successfully at {Path}", app.Name, installPath);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to install app {AppName}", app.Name);
            return false;
        }
    }

    public async Task<bool> UninstallAppAsync(string appId)
    {
        try
        {
            var installedApp = await _context.InstalledApps
                .FirstOrDefaultAsync(a => a.ApplicationId == appId);

            if (installedApp == null)
            {
                _logger.LogWarning("App {AppId} not found in installed apps", appId);
                return false;
            }

            // Stop if running
            if (installedApp.IsRunning)
            {
                await StopAppAsync(appId);
            }

            // Delete files
            if (Directory.Exists(installedApp.InstallPath))
            {
                Directory.Delete(installedApp.InstallPath, true);
            }

            // Remove from database
            _context.InstalledApps.Remove(installedApp);

            // Update application installed status
            var app = await _context.Applications.FindAsync(appId);
            if (app != null)
            {
                app.IsInstalled = false;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("App {AppName} uninstalled successfully", installedApp.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to uninstall app {AppId}", appId);
            return false;
        }
    }

    public async Task<bool> LaunchAppAsync(string appId)
    {
        try
        {
            var installedApp = await _context.InstalledApps
                .FirstOrDefaultAsync(a => a.ApplicationId == appId);

            if (installedApp == null)
            {
                _logger.LogWarning("App {AppId} not found", appId);
                return false;
            }

            if (!File.Exists(installedApp.ExecutablePath))
            {
                _logger.LogError("Executable not found: {Path}", installedApp.ExecutablePath);
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = installedApp.ExecutablePath,
                Arguments = installedApp.LaunchArguments,
                WorkingDirectory = installedApp.InstallPath,
                UseShellExecute = true
            };

            var process = Process.Start(startInfo);
            if (process != null)
            {
                installedApp.IsRunning = true;
                installedApp.ProcessId = process.Id;
                installedApp.LastLaunchedAt = DateTime.Now;
                installedApp.LaunchCount++;

                var session = new AppLaunchSession
                {
                    StartTime = DateTime.Now
                };
                installedApp.LaunchHistory.Add(session);

                _runningProcesses[appId] = process;

                // Monitor process exit
                process.EnableRaisingEvents = true;
                process.Exited += async (s, e) => await OnAppExited(appId, session);

                await _context.SaveChangesAsync();

                _logger.LogInformation("App {AppName} launched with PID {Pid}", installedApp.Name, process.Id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to launch app {AppId}", appId);
            return false;
        }
    }

    private async Task OnAppExited(string appId, AppLaunchSession session)
    {
        try
        {
            session.EndTime = DateTime.Now;

            var installedApp = await _context.InstalledApps
                .FirstOrDefaultAsync(a => a.ApplicationId == appId);

            if (installedApp != null)
            {
                installedApp.IsRunning = false;
                installedApp.ProcessId = null;
                installedApp.TotalPlayTime += session.Duration;
                await _context.SaveChangesAsync();
            }

            _runningProcesses.Remove(appId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling app exit for {AppId}", appId);
        }
    }

    public async Task<bool> StopAppAsync(string appId)
    {
        try
        {
            if (_runningProcesses.TryGetValue(appId, out var process))
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit(5000);
                }
                _runningProcesses.Remove(appId);
            }

            var installedApp = await _context.InstalledApps
                .FirstOrDefaultAsync(a => a.ApplicationId == appId);

            if (installedApp != null)
            {
                installedApp.IsRunning = false;
                installedApp.ProcessId = null;
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to stop app {AppId}", appId);
            return false;
        }
    }

    #endregion

    #region Update Operations

    public async Task<IEnumerable<AppUpdate>> CheckForUpdatesAsync()
    {
        var updates = new List<AppUpdate>();

        var installedApps = await _context.InstalledApps.ToListAsync();

        foreach (var installed in installedApps)
        {
            var app = await _context.Applications.FindAsync(installed.ApplicationId);
            if (app != null && app.Version != installed.InstalledVersion)
            {
                updates.Add(new AppUpdate
                {
                    ApplicationId = app.Id,
                    ApplicationName = app.Name,
                    CurrentVersion = installed.InstalledVersion,
                    NewVersion = app.Version,
                    ReleaseDate = app.LastUpdated,
                    DownloadUrl = app.DownloadUrl,
                    UpdateSizeBytes = app.SizeInBytes
                });

                installed.LatestVersion = app.Version;
            }
        }

        await _context.SaveChangesAsync();
        return updates;
    }

    public async Task<bool> UpdateAppAsync(string appId)
    {
        // Implementation would download and apply update
        _logger.LogInformation("Updating app {AppId}", appId);
        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateAllAppsAsync()
    {
        var updates = await CheckForUpdatesAsync();
        foreach (var update in updates)
        {
            await UpdateAppAsync(update.ApplicationId);
        }
        return true;
    }

    #endregion

    #region Favorites and Wishlist

    public async Task AddToFavoritesAsync(string appId)
    {
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user != null && !user.FavoriteAppIds.Contains(appId))
        {
            user.FavoriteAppIds.Add(appId);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromFavoritesAsync(string appId)
    {
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user != null)
        {
            user.FavoriteAppIds.Remove(appId);
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddToWishlistAsync(string appId)
    {
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user != null && !user.WishlistAppIds.Contains(appId))
        {
            user.WishlistAppIds.Add(appId);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RemoveFromWishlistAsync(string appId)
    {
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user != null)
        {
            user.WishlistAppIds.Remove(appId);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Application>> GetWishlistAsync()
    {
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user == null) return Enumerable.Empty<Application>();

        return await _context.Applications
            .Where(a => user.WishlistAppIds.Contains(a.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<InstalledApp>> GetFavoriteAppsAsync()
    {
        var user = await _context.Users.FirstOrDefaultAsync();
        if (user == null) return Enumerable.Empty<InstalledApp>();

        return await _context.InstalledApps
            .Where(a => user.FavoriteAppIds.Contains(a.ApplicationId))
            .ToListAsync();
    }

    #endregion

    #region Reviews

    public async Task<IEnumerable<Review>> GetAppReviewsAsync(string appId)
    {
        return await _context.Reviews
            .Where(r => r.ApplicationId == appId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> SubmitReviewAsync(Review review)
    {
        try
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();

            // Update app rating
            var reviews = await _context.Reviews
                .Where(r => r.ApplicationId == review.ApplicationId)
                .ToListAsync();

            var app = await _context.Applications.FindAsync(review.ApplicationId);
            if (app != null && reviews.Any())
            {
                app.Rating = reviews.Average(r => r.Rating);
                app.ReviewCount = reviews.Count;
                await _context.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to submit review");
            return false;
        }
    }

    #endregion
}
