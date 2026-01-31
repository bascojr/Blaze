using System.ComponentModel.DataAnnotations;

namespace Blaze.Core.Models;

/// <summary>
/// Represents an available update for an installed application
/// </summary>
public class AppUpdate
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ApplicationId { get; set; } = string.Empty;

    public string ApplicationName { get; set; } = string.Empty;

    public string CurrentVersion { get; set; } = string.Empty;

    public string NewVersion { get; set; } = string.Empty;

    public DateTime ReleaseDate { get; set; }

    public string ReleaseNotes { get; set; } = string.Empty;

    public List<string> ChangeLog { get; set; } = new();

    public long UpdateSizeBytes { get; set; }

    public string UpdateSizeFormatted => FormatSize(UpdateSizeBytes);

    public bool IsMandatory { get; set; } = false;

    public bool IsSecurityUpdate { get; set; } = false;

    public UpdatePriority Priority { get; set; } = UpdatePriority.Normal;

    public string DownloadUrl { get; set; } = string.Empty;

    public string Checksum { get; set; } = string.Empty;

    public bool RequiresRestart { get; set; } = false;

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

public enum UpdatePriority
{
    Low,
    Normal,
    High,
    Critical
}
