using Blaze.Core.Models;

namespace Blaze.Core.Services;

/// <summary>
/// Service interface for managing application settings
/// </summary>
public interface ISettingsService
{
    // Settings operations
    Task<BlazeSettings> GetSettingsAsync();
    Task SaveSettingsAsync(BlazeSettings settings);

    // Individual setting sections
    Task<GeneralSettings> GetGeneralSettingsAsync();
    Task SaveGeneralSettingsAsync(GeneralSettings settings);

    Task<DownloadSettings> GetDownloadSettingsAsync();
    Task SaveDownloadSettingsAsync(DownloadSettings settings);

    Task<NotificationSettings> GetNotificationSettingsAsync();
    Task SaveNotificationSettingsAsync(NotificationSettings settings);

    Task<InterfaceSettings> GetInterfaceSettingsAsync();
    Task SaveInterfaceSettingsAsync(InterfaceSettings settings);

    Task<LibrarySettings> GetLibrarySettingsAsync();
    Task SaveLibrarySettingsAsync(LibrarySettings settings);

    // Quick access settings
    Task<string> GetDefaultInstallPathAsync();
    Task SetDefaultInstallPathAsync(string path);

    Task<string> GetThemeAsync();
    Task SetThemeAsync(string theme);

    // Reset
    Task ResetToDefaultsAsync();
}
