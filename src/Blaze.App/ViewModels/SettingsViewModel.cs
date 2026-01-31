using Blaze.Core.Models;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the Settings view
/// </summary>
public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    private readonly IAuthService _authService;

    // General Settings
    [ObservableProperty]
    private bool _runOnStartup;

    [ObservableProperty]
    private bool _startMinimized;

    [ObservableProperty]
    private bool _minimizeToTray;

    [ObservableProperty]
    private bool _closeToTray;

    [ObservableProperty]
    private bool _checkForUpdatesAutomatically;

    [ObservableProperty]
    private string _language = "en-US";

    // Download Settings
    [ObservableProperty]
    private string _defaultInstallLocation = @"C:\Program Files\Blaze Apps";

    [ObservableProperty]
    private int _maxConcurrentDownloads = 3;

    [ObservableProperty]
    private int _speedLimitMbps;

    [ObservableProperty]
    private bool _autoInstallAfterDownload = true;

    [ObservableProperty]
    private bool _verifyDownloads = true;

    // Notification Settings
    [ObservableProperty]
    private bool _enableNotifications = true;

    [ObservableProperty]
    private bool _notifyOnDownloadComplete = true;

    [ObservableProperty]
    private bool _notifyOnUpdateAvailable = true;

    [ObservableProperty]
    private bool _playSounds = true;

    // Interface Settings
    [ObservableProperty]
    private string _theme = "Dark";

    [ObservableProperty]
    private string _accentColor = "#1B9AAA";

    [ObservableProperty]
    private bool _showAnimations = true;

    [ObservableProperty]
    private double _uiScale = 1.0;

    [ObservableProperty]
    private bool _compactMode;

    // Library Settings
    [ObservableProperty]
    private bool _trackUsageTime = true;

    [ObservableProperty]
    private bool _createDesktopShortcuts = true;

    [ObservableProperty]
    private bool _createStartMenuShortcuts = true;

    [ObservableProperty]
    private string _defaultLibraryView = "Grid";

    public string[] AvailableLanguages { get; } = { "en-US", "es-ES", "fr-FR", "de-DE", "ja-JP", "zh-CN" };
    public string[] AvailableThemes { get; } = { "Dark", "Light", "System" };
    public string[] AvailableViews { get; } = { "Grid", "List", "Details" };
    public int[] ConcurrentDownloadOptions { get; } = { 1, 2, 3, 4, 5 };

    public SettingsViewModel(ISettingsService settingsService, IAuthService authService)
    {
        _settingsService = settingsService;
        _authService = authService;
    }

    public override async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            var settings = await _settingsService.GetSettingsAsync();

            // General
            RunOnStartup = settings.General.RunOnStartup;
            StartMinimized = settings.General.StartMinimized;
            MinimizeToTray = settings.General.MinimizeToTray;
            CloseToTray = settings.General.CloseToTray;
            CheckForUpdatesAutomatically = settings.General.CheckForUpdatesAutomatically;
            Language = settings.General.Language;

            // Downloads
            DefaultInstallLocation = settings.Downloads.DefaultInstallLocation;
            MaxConcurrentDownloads = settings.Downloads.MaxConcurrentDownloads;
            SpeedLimitMbps = settings.Downloads.SpeedLimitMbps;
            AutoInstallAfterDownload = settings.Downloads.AutoInstallAfterDownload;
            VerifyDownloads = settings.Downloads.VerifyDownloads;

            // Notifications
            EnableNotifications = settings.Notifications.EnableNotifications;
            NotifyOnDownloadComplete = settings.Notifications.NotifyOnDownloadComplete;
            NotifyOnUpdateAvailable = settings.Notifications.NotifyOnUpdateAvailable;
            PlaySounds = settings.Notifications.PlaySounds;

            // Interface
            Theme = settings.Interface.Theme;
            AccentColor = settings.Interface.AccentColor;
            ShowAnimations = settings.Interface.ShowAnimations;
            UiScale = settings.Interface.UIScale;
            CompactMode = settings.Interface.CompactMode;
            DefaultLibraryView = settings.Interface.DefaultLibraryView.ToString();

            // Library
            TrackUsageTime = settings.Library.TrackUsageTime;
            CreateDesktopShortcuts = settings.Library.CreateDesktopShortcuts;
            CreateStartMenuShortcuts = settings.Library.CreateStartMenuShortcuts;
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        var settings = new BlazeSettings
        {
            General = new GeneralSettings
            {
                RunOnStartup = RunOnStartup,
                StartMinimized = StartMinimized,
                MinimizeToTray = MinimizeToTray,
                CloseToTray = CloseToTray,
                CheckForUpdatesAutomatically = CheckForUpdatesAutomatically,
                Language = Language
            },
            Downloads = new DownloadSettings
            {
                DefaultInstallLocation = DefaultInstallLocation,
                MaxConcurrentDownloads = MaxConcurrentDownloads,
                SpeedLimitMbps = SpeedLimitMbps,
                AutoInstallAfterDownload = AutoInstallAfterDownload,
                VerifyDownloads = VerifyDownloads
            },
            Notifications = new NotificationSettings
            {
                EnableNotifications = EnableNotifications,
                NotifyOnDownloadComplete = NotifyOnDownloadComplete,
                NotifyOnUpdateAvailable = NotifyOnUpdateAvailable,
                PlaySounds = PlaySounds
            },
            Interface = new InterfaceSettings
            {
                Theme = Theme,
                AccentColor = AccentColor,
                ShowAnimations = ShowAnimations,
                UIScale = UiScale,
                CompactMode = CompactMode,
                DefaultLibraryView = Enum.Parse<LibraryViewMode>(DefaultLibraryView)
            },
            Library = new LibrarySettings
            {
                TrackUsageTime = TrackUsageTime,
                CreateDesktopShortcuts = CreateDesktopShortcuts,
                CreateStartMenuShortcuts = CreateStartMenuShortcuts
            }
        };

        await _settingsService.SaveSettingsAsync(settings);
    }

    [RelayCommand]
    private async Task ResetToDefaults()
    {
        await _settingsService.ResetToDefaultsAsync();
        await InitializeAsync();
    }

    [RelayCommand]
    private void BrowseInstallLocation()
    {
        // Would open folder browser dialog
        // For now, just use a default
    }

    [RelayCommand]
    private void OpenDataFolder()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var blazeFolder = System.IO.Path.Combine(appData, "Blaze");
        System.Diagnostics.Process.Start("explorer.exe", blazeFolder);
    }

    [RelayCommand]
    private async Task ClearCache()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var cacheFolder = System.IO.Path.Combine(appData, "Blaze", "Cache");
        if (System.IO.Directory.Exists(cacheFolder))
        {
            System.IO.Directory.Delete(cacheFolder, true);
            System.IO.Directory.CreateDirectory(cacheFolder);
        }
        await Task.CompletedTask;
    }
}
