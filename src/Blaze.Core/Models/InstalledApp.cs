using System.ComponentModel.DataAnnotations;

namespace Blaze.Core.Models;

/// <summary>
/// Represents an application installed on the user's system
/// </summary>
public class InstalledApp
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ApplicationId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string InstalledVersion { get; set; } = string.Empty;

    public string LatestVersion { get; set; } = string.Empty;

    public bool HasUpdate => !string.IsNullOrEmpty(LatestVersion) &&
                             InstalledVersion != LatestVersion;

    public string InstallPath { get; set; } = string.Empty;

    public string ExecutablePath { get; set; } = string.Empty;

    public long InstalledSizeBytes { get; set; }

    public string InstalledSizeFormatted => FormatSize(InstalledSizeBytes);

    public DateTime InstalledAt { get; set; } = DateTime.Now;

    public DateTime LastLaunchedAt { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public TimeSpan TotalPlayTime { get; set; } = TimeSpan.Zero;

    public string TotalPlayTimeFormatted
    {
        get
        {
            if (TotalPlayTime.TotalHours >= 1)
                return $"{(int)TotalPlayTime.TotalHours} hours";
            if (TotalPlayTime.TotalMinutes >= 1)
                return $"{(int)TotalPlayTime.TotalMinutes} minutes";
            return "Less than a minute";
        }
    }

    public int LaunchCount { get; set; } = 0;

    public string IconPath { get; set; } = string.Empty;

    public bool IsRunning { get; set; } = false;

    public int? ProcessId { get; set; }

    public bool AutoUpdate { get; set; } = true;

    public bool CreateDesktopShortcut { get; set; } = true;

    public bool CreateStartMenuShortcut { get; set; } = true;

    public string LaunchArguments { get; set; } = string.Empty;

    public List<AppLaunchSession> LaunchHistory { get; set; } = new();

    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}

/// <summary>
/// Represents a single launch session for an app
/// </summary>
public class AppLaunchSession
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public TimeSpan Duration => EndTime.HasValue
        ? EndTime.Value - StartTime
        : DateTime.Now - StartTime;
}
