using System.Collections.Concurrent;
using Blaze.Core.Data;
using Blaze.Core.Events;
using Blaze.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blaze.Core.Services;

/// <summary>
/// Implementation of the download service
/// </summary>
public class DownloadService : IDownloadService
{
    private readonly BlazeDbContext _context;
    private readonly ILogger<DownloadService> _logger;
    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _downloadTokens = new();
    private readonly SemaphoreSlim _downloadSemaphore;
    private int _maxConcurrent = 3;
    private int _speedLimitMbps = 0;

    public event EventHandler<DownloadProgressEventArgs>? DownloadProgressChanged;
    public event EventHandler<DownloadCompletedEventArgs>? DownloadCompleted;
    public event EventHandler<InstallProgressEventArgs>? InstallProgressChanged;

    public DownloadService(BlazeDbContext context, ILogger<DownloadService> logger)
    {
        _context = context;
        _logger = logger;
        _httpClient = new HttpClient();
        _downloadSemaphore = new SemaphoreSlim(_maxConcurrent, _maxConcurrent);
    }

    public async Task<Download> QueueDownloadAsync(Application app, string destinationPath)
    {
        var download = new Download
        {
            ApplicationId = app.Id,
            ApplicationName = app.Name,
            TotalBytes = app.SizeInBytes,
            DestinationPath = destinationPath,
            TempFilePath = Path.Combine(Path.GetTempPath(), $"blaze_{app.Id}.tmp"),
            Status = DownloadStatus.Queued
        };

        await _context.Downloads.AddAsync(download);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Download queued for {AppName}", app.Name);

        // Auto-start if under limit
        _ = StartDownloadAsync(download.Id);

        return download;
    }

    public async Task<bool> StartDownloadAsync(string downloadId)
    {
        var download = await _context.Downloads.FindAsync(downloadId);
        if (download == null) return false;

        if (download.Status == DownloadStatus.Downloading) return true;

        var cts = new CancellationTokenSource();
        _downloadTokens[downloadId] = cts;

        _ = Task.Run(async () => await ExecuteDownloadAsync(download, cts.Token));

        return true;
    }

    private async Task ExecuteDownloadAsync(Download download, CancellationToken cancellationToken)
    {
        await _downloadSemaphore.WaitAsync(cancellationToken);

        try
        {
            download.Status = DownloadStatus.Downloading;
            download.StartedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            var app = await _context.Applications.FindAsync(download.ApplicationId);
            if (app == null || string.IsNullOrEmpty(app.DownloadUrl))
            {
                await MarkDownloadFailed(download, "Application or download URL not found");
                return;
            }

            using var response = await _httpClient.GetAsync(app.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? download.TotalBytes;
            download.TotalBytes = totalBytes;

            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            await using var fileStream = new FileStream(download.TempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            var totalBytesRead = 0L;
            var lastReportTime = DateTime.Now;
            var bytesReadSinceLastReport = 0L;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var bytesRead = await contentStream.ReadAsync(buffer, cancellationToken);
                if (bytesRead == 0) break;

                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);

                totalBytesRead += bytesRead;
                bytesReadSinceLastReport += bytesRead;
                download.DownloadedBytes = totalBytesRead;

                // Apply speed limit
                if (_speedLimitMbps > 0)
                {
                    var targetBytesPerSecond = _speedLimitMbps * 1024 * 1024 / 8;
                    var elapsed = (DateTime.Now - lastReportTime).TotalSeconds;
                    if (elapsed > 0)
                    {
                        var currentSpeed = bytesReadSinceLastReport / elapsed;
                        if (currentSpeed > targetBytesPerSecond)
                        {
                            var delay = (int)((bytesReadSinceLastReport / targetBytesPerSecond - elapsed) * 1000);
                            if (delay > 0) await Task.Delay(delay, cancellationToken);
                        }
                    }
                }

                // Report progress every 100ms
                if ((DateTime.Now - lastReportTime).TotalMilliseconds >= 100)
                {
                    var elapsed = (DateTime.Now - lastReportTime).TotalSeconds;
                    var speed = elapsed > 0 ? bytesReadSinceLastReport / elapsed : 0;
                    download.SpeedBytesPerSecond = speed;

                    DownloadProgressChanged?.Invoke(this, new DownloadProgressEventArgs(
                        download, totalBytesRead, totalBytes, speed));

                    await _context.SaveChangesAsync();

                    lastReportTime = DateTime.Now;
                    bytesReadSinceLastReport = 0;
                }
            }

            // Download complete - move to destination
            download.Status = DownloadStatus.Installing;
            await _context.SaveChangesAsync();

            InstallProgressChanged?.Invoke(this, new InstallProgressEventArgs(
                download.ApplicationId, download.ApplicationName, InstallStage.Extracting, 0, "Extracting files..."));

            // Move temp file to destination
            Directory.CreateDirectory(download.DestinationPath);
            var destFile = Path.Combine(download.DestinationPath, Path.GetFileName(download.TempFilePath));
            File.Move(download.TempFilePath, destFile, true);

            InstallProgressChanged?.Invoke(this, new InstallProgressEventArgs(
                download.ApplicationId, download.ApplicationName, InstallStage.Completed, 100, "Installation complete"));

            download.Status = DownloadStatus.Completed;
            download.CompletedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(download, true));

            _logger.LogInformation("Download completed: {AppName}", download.ApplicationName);
        }
        catch (OperationCanceledException)
        {
            download.Status = DownloadStatus.Paused;
            await _context.SaveChangesAsync();
            _logger.LogInformation("Download paused: {AppName}", download.ApplicationName);
        }
        catch (Exception ex)
        {
            await MarkDownloadFailed(download, ex.Message);
        }
        finally
        {
            _downloadTokens.TryRemove(download.Id, out _);
            _downloadSemaphore.Release();
        }
    }

    private async Task MarkDownloadFailed(Download download, string error)
    {
        download.Status = DownloadStatus.Failed;
        download.ErrorMessage = error;
        download.RetryCount++;
        await _context.SaveChangesAsync();

        DownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(download, false, error));
        _logger.LogError("Download failed: {AppName} - {Error}", download.ApplicationName, error);
    }

    public async Task<bool> PauseDownloadAsync(string downloadId)
    {
        if (_downloadTokens.TryGetValue(downloadId, out var cts))
        {
            cts.Cancel();
            return true;
        }

        var download = await _context.Downloads.FindAsync(downloadId);
        if (download != null)
        {
            download.Status = DownloadStatus.Paused;
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> ResumeDownloadAsync(string downloadId)
    {
        var download = await _context.Downloads.FindAsync(downloadId);
        if (download == null) return false;

        if (download.Status == DownloadStatus.Paused)
        {
            return await StartDownloadAsync(downloadId);
        }

        return false;
    }

    public async Task<bool> CancelDownloadAsync(string downloadId)
    {
        if (_downloadTokens.TryGetValue(downloadId, out var cts))
        {
            cts.Cancel();
        }

        var download = await _context.Downloads.FindAsync(downloadId);
        if (download != null)
        {
            download.Status = DownloadStatus.Cancelled;
            await _context.SaveChangesAsync();

            // Cleanup temp file
            if (File.Exists(download.TempFilePath))
            {
                File.Delete(download.TempFilePath);
            }
        }

        return true;
    }

    public async Task<bool> RetryDownloadAsync(string downloadId)
    {
        var download = await _context.Downloads.FindAsync(downloadId);
        if (download == null) return false;

        if (download.Status == DownloadStatus.Failed && download.RetryCount < download.MaxRetries)
        {
            download.Status = DownloadStatus.Queued;
            download.ErrorMessage = string.Empty;
            await _context.SaveChangesAsync();
            return await StartDownloadAsync(downloadId);
        }

        return false;
    }

    public async Task<DownloadQueue> GetQueueAsync()
    {
        var downloads = await _context.Downloads.ToListAsync();

        return new DownloadQueue
        {
            ActiveDownloads = downloads.Where(d => d.Status == DownloadStatus.Downloading).ToList(),
            QueuedDownloads = downloads.Where(d => d.Status == DownloadStatus.Queued || d.Status == DownloadStatus.Paused).ToList(),
            CompletedDownloads = downloads.Where(d => d.Status == DownloadStatus.Completed).ToList(),
            MaxConcurrent = _maxConcurrent
        };
    }

    public async Task<Download?> GetDownloadAsync(string downloadId)
    {
        return await _context.Downloads.FindAsync(downloadId);
    }

    public async Task<IEnumerable<Download>> GetActiveDownloadsAsync()
    {
        return await _context.Downloads
            .Where(d => d.Status == DownloadStatus.Downloading)
            .ToListAsync();
    }

    public async Task<IEnumerable<Download>> GetCompletedDownloadsAsync()
    {
        return await _context.Downloads
            .Where(d => d.Status == DownloadStatus.Completed)
            .OrderByDescending(d => d.CompletedAt)
            .ToListAsync();
    }

    public async Task ClearCompletedDownloadsAsync()
    {
        var completed = await _context.Downloads
            .Where(d => d.Status == DownloadStatus.Completed)
            .ToListAsync();

        _context.Downloads.RemoveRange(completed);
        await _context.SaveChangesAsync();
    }

    public async Task PauseAllDownloadsAsync()
    {
        foreach (var cts in _downloadTokens.Values)
        {
            cts.Cancel();
        }

        var activeDownloads = await _context.Downloads
            .Where(d => d.Status == DownloadStatus.Downloading)
            .ToListAsync();

        foreach (var download in activeDownloads)
        {
            download.Status = DownloadStatus.Paused;
        }

        await _context.SaveChangesAsync();
    }

    public async Task ResumeAllDownloadsAsync()
    {
        var pausedDownloads = await _context.Downloads
            .Where(d => d.Status == DownloadStatus.Paused)
            .ToListAsync();

        foreach (var download in pausedDownloads)
        {
            await StartDownloadAsync(download.Id);
        }
    }

    public Task SetMaxConcurrentDownloadsAsync(int count)
    {
        _maxConcurrent = count;
        return Task.CompletedTask;
    }

    public Task SetSpeedLimitAsync(int mbps)
    {
        _speedLimitMbps = mbps;
        return Task.CompletedTask;
    }

    public async Task<long> GetTotalDownloadedBytesAsync()
    {
        return await _context.Downloads
            .SumAsync(d => d.DownloadedBytes);
    }

    public async Task<double> GetCurrentSpeedAsync()
    {
        var active = await GetActiveDownloadsAsync();
        return active.Sum(d => d.SpeedBytesPerSecond);
    }
}
