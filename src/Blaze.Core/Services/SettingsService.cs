using Blaze.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Blaze.Core.Services;

/// <summary>
/// Implementation of the settings service
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;
    private readonly string _settingsPath;
    private BlazeSettings? _cachedSettings;

    public SettingsService(ILogger<SettingsService> logger)
    {
        _logger = logger;

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var blazeFolder = Path.Combine(appData, "Blaze");
        Directory.CreateDirectory(blazeFolder);
        _settingsPath = Path.Combine(blazeFolder, "settings.json");
    }

    public async Task<BlazeSettings> GetSettingsAsync()
    {
        if (_cachedSettings != null) return _cachedSettings;

        if (File.Exists(_settingsPath))
        {
            try
            {
                var json = await File.ReadAllTextAsync(_settingsPath);
                _cachedSettings = JsonConvert.DeserializeObject<BlazeSettings>(json) ?? new BlazeSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load settings");
                _cachedSettings = new BlazeSettings();
            }
        }
        else
        {
            _cachedSettings = new BlazeSettings();
            await SaveSettingsAsync(_cachedSettings);
        }

        return _cachedSettings;
    }

    public async Task SaveSettingsAsync(BlazeSettings settings)
    {
        try
        {
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            await File.WriteAllTextAsync(_settingsPath, json);
            _cachedSettings = settings;
            _logger.LogInformation("Settings saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save settings");
            throw;
        }
    }

    public async Task<GeneralSettings> GetGeneralSettingsAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.General;
    }

    public async Task SaveGeneralSettingsAsync(GeneralSettings settings)
    {
        var allSettings = await GetSettingsAsync();
        allSettings.General = settings;
        await SaveSettingsAsync(allSettings);
    }

    public async Task<DownloadSettings> GetDownloadSettingsAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Downloads;
    }

    public async Task SaveDownloadSettingsAsync(DownloadSettings settings)
    {
        var allSettings = await GetSettingsAsync();
        allSettings.Downloads = settings;
        await SaveSettingsAsync(allSettings);
    }

    public async Task<NotificationSettings> GetNotificationSettingsAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Notifications;
    }

    public async Task SaveNotificationSettingsAsync(NotificationSettings settings)
    {
        var allSettings = await GetSettingsAsync();
        allSettings.Notifications = settings;
        await SaveSettingsAsync(allSettings);
    }

    public async Task<InterfaceSettings> GetInterfaceSettingsAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Interface;
    }

    public async Task SaveInterfaceSettingsAsync(InterfaceSettings settings)
    {
        var allSettings = await GetSettingsAsync();
        allSettings.Interface = settings;
        await SaveSettingsAsync(allSettings);
    }

    public async Task<LibrarySettings> GetLibrarySettingsAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Library;
    }

    public async Task SaveLibrarySettingsAsync(LibrarySettings settings)
    {
        var allSettings = await GetSettingsAsync();
        allSettings.Library = settings;
        await SaveSettingsAsync(allSettings);
    }

    public async Task<string> GetDefaultInstallPathAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Downloads.DefaultInstallLocation;
    }

    public async Task SetDefaultInstallPathAsync(string path)
    {
        var settings = await GetSettingsAsync();
        settings.Downloads.DefaultInstallLocation = path;
        await SaveSettingsAsync(settings);
    }

    public async Task<string> GetThemeAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.Interface.Theme;
    }

    public async Task SetThemeAsync(string theme)
    {
        var settings = await GetSettingsAsync();
        settings.Interface.Theme = theme;
        await SaveSettingsAsync(settings);
    }

    public async Task ResetToDefaultsAsync()
    {
        _cachedSettings = new BlazeSettings();
        await SaveSettingsAsync(_cachedSettings);
        _logger.LogInformation("Settings reset to defaults");
    }
}
