using System.ComponentModel.DataAnnotations;

namespace Blaze.Core.Models;

/// <summary>
/// Represents a user account in Blaze
/// </summary>
public class User
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string AvatarUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;

    public UserPreferences Preferences { get; set; } = new();

    public List<string> OwnedAppIds { get; set; } = new();

    public List<string> WishlistAppIds { get; set; } = new();

    public List<string> FavoriteAppIds { get; set; } = new();

    public bool IsOnline { get; set; } = true;

    public UserStatus Status { get; set; } = UserStatus.Online;

    public string StatusMessage { get; set; } = string.Empty;
}

public enum UserStatus
{
    Online,
    Away,
    Busy,
    Invisible,
    Offline
}

/// <summary>
/// User preferences and settings
/// </summary>
public class UserPreferences
{
    public string Theme { get; set; } = "Dark";

    public bool AutoStartWithWindows { get; set; } = false;

    public bool MinimizeToTray { get; set; } = true;

    public bool ShowNotifications { get; set; } = true;

    public bool AutoUpdateApps { get; set; } = false;

    public string DefaultInstallPath { get; set; } = @"C:\Program Files\Blaze Apps";

    public int MaxConcurrentDownloads { get; set; } = 3;

    public int DownloadSpeedLimitMbps { get; set; } = 0; // 0 = unlimited

    public bool EnableBetaUpdates { get; set; } = false;

    public string Language { get; set; } = "en-US";

    public bool SaveLoginCredentials { get; set; } = true;
}
