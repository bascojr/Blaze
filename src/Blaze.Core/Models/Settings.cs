namespace Blaze.Core.Models;

/// <summary>
/// Application-wide settings
/// </summary>
public class BlazeSettings
{
    public GeneralSettings General { get; set; } = new();

    public DownloadSettings Downloads { get; set; } = new();

    public NotificationSettings Notifications { get; set; } = new();

    public InterfaceSettings Interface { get; set; } = new();

    public LibrarySettings Library { get; set; } = new();
}

public class GeneralSettings
{
    public bool RunOnStartup { get; set; } = false;

    public bool StartMinimized { get; set; } = false;

    public bool MinimizeToTray { get; set; } = true;

    public bool CloseToTray { get; set; } = true;

    public bool CheckForUpdatesAutomatically { get; set; } = true;

    public bool EnableBetaProgram { get; set; } = false;

    public string Language { get; set; } = "en-US";

    public bool EnableAnalytics { get; set; } = true;
}

public class DownloadSettings
{
    public string DefaultInstallLocation { get; set; } = @"C:\Program Files\Blaze Apps";

    public int MaxConcurrentDownloads { get; set; } = 3;

    public int SpeedLimitMbps { get; set; } = 0; // 0 = unlimited

    public bool ThrottleWhileGaming { get; set; } = true;

    public bool AutoInstallAfterDownload { get; set; } = true;

    public bool VerifyDownloads { get; set; } = true;

    public bool DeleteArchivesAfterInstall { get; set; } = true;

    public bool AllowBackgroundDownloads { get; set; } = true;

    public ScheduledDownloadTime ScheduledDownloads { get; set; } = new();
}

public class ScheduledDownloadTime
{
    public bool Enabled { get; set; } = false;

    public TimeSpan StartTime { get; set; } = new(2, 0, 0); // 2 AM

    public TimeSpan EndTime { get; set; } = new(6, 0, 0); // 6 AM
}

public class NotificationSettings
{
    public bool EnableNotifications { get; set; } = true;

    public bool NotifyOnDownloadComplete { get; set; } = true;

    public bool NotifyOnUpdateAvailable { get; set; } = true;

    public bool NotifyOnAppLaunch { get; set; } = false;

    public bool PlaySounds { get; set; } = true;

    public bool ShowInActionCenter { get; set; } = true;
}

public class InterfaceSettings
{
    public string Theme { get; set; } = "Dark";

    public string AccentColor { get; set; } = "#1B9AAA"; // Blaze teal

    public bool ShowAnimations { get; set; } = true;

    public bool HighContrastMode { get; set; } = false;

    public double UIScale { get; set; } = 1.0;

    public bool CompactMode { get; set; } = false;

    public LibraryViewMode DefaultLibraryView { get; set; } = LibraryViewMode.Grid;

    public bool ShowAppSizeInLibrary { get; set; } = true;

    public bool ShowLastPlayedInLibrary { get; set; } = true;
}

public enum LibraryViewMode
{
    Grid,
    List,
    Details
}

public class LibrarySettings
{
    public bool ShowHiddenApps { get; set; } = false;

    public bool GroupByCategory { get; set; } = true;

    public SortOption DefaultSort { get; set; } = SortOption.Name;

    public bool SortAscending { get; set; } = true;

    public bool TrackUsageTime { get; set; } = true;

    public bool CreateDesktopShortcuts { get; set; } = true;

    public bool CreateStartMenuShortcuts { get; set; } = true;
}

public enum SortOption
{
    Name,
    RecentlyPlayed,
    InstallDate,
    Size,
    PlayTime
}
