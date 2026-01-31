using Blaze.Core.Models;

namespace Blaze.Core.Services;

/// <summary>
/// Service interface for managing applications
/// </summary>
public interface IAppService
{
    // Store/Catalog operations
    Task<IEnumerable<Application>> GetAllAppsAsync();
    Task<IEnumerable<Application>> GetFeaturedAppsAsync();
    Task<IEnumerable<Application>> GetPopularAppsAsync(int count = 10);
    Task<IEnumerable<Application>> GetNewReleasesAsync(int count = 10);
    Task<IEnumerable<Application>> GetAppsByCategoryAsync(string categoryId);
    Task<IEnumerable<Application>> SearchAppsAsync(string query);
    Task<Application?> GetAppByIdAsync(string appId);
    Task<IEnumerable<Category>> GetCategoriesAsync();

    // Library operations
    Task<IEnumerable<InstalledApp>> GetInstalledAppsAsync();
    Task<InstalledApp?> GetInstalledAppAsync(string appId);
    Task<bool> IsAppInstalledAsync(string appId);

    // App management
    Task<bool> InstallAppAsync(Application app, string installPath);
    Task<bool> UninstallAppAsync(string appId);
    Task<bool> LaunchAppAsync(string appId);
    Task<bool> StopAppAsync(string appId);

    // Update operations
    Task<IEnumerable<AppUpdate>> CheckForUpdatesAsync();
    Task<bool> UpdateAppAsync(string appId);
    Task<bool> UpdateAllAppsAsync();

    // Favorites and wishlist
    Task AddToFavoritesAsync(string appId);
    Task RemoveFromFavoritesAsync(string appId);
    Task AddToWishlistAsync(string appId);
    Task RemoveFromWishlistAsync(string appId);
    Task<IEnumerable<Application>> GetWishlistAsync();
    Task<IEnumerable<InstalledApp>> GetFavoriteAppsAsync();

    // Reviews
    Task<IEnumerable<Review>> GetAppReviewsAsync(string appId);
    Task<bool> SubmitReviewAsync(Review review);
}
