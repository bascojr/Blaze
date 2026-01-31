using System.ComponentModel.DataAnnotations;

namespace Blaze.Core.Models;

/// <summary>
/// Represents an application available in the Blaze store
/// </summary>
public class Application
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ShortDescription { get; set; } = string.Empty;

    public string Developer { get; set; } = string.Empty;

    public string Publisher { get; set; } = string.Empty;

    public string Version { get; set; } = "1.0.0";

    public DateTime ReleaseDate { get; set; } = DateTime.Now;

    public DateTime LastUpdated { get; set; } = DateTime.Now;

    public decimal Price { get; set; } = 0;

    public bool IsFree => Price == 0;

    public long SizeInBytes { get; set; }

    public string SizeFormatted => FormatSize(SizeInBytes);

    public string DownloadUrl { get; set; } = string.Empty;

    public string ExecutablePath { get; set; } = string.Empty;

    public string IconUrl { get; set; } = string.Empty;

    public string HeaderImageUrl { get; set; } = string.Empty;

    public List<string> ScreenshotUrls { get; set; } = new();

    public string CategoryId { get; set; } = string.Empty;

    public Category? Category { get; set; }

    public List<string> Tags { get; set; } = new();

    public double Rating { get; set; } = 0;

    public int ReviewCount { get; set; } = 0;

    public int DownloadCount { get; set; } = 0;

    public string MinimumOsVersion { get; set; } = "Windows 10";

    public string SupportedArchitectures { get; set; } = "x64";

    public bool RequiresAdmin { get; set; } = false;

    public string Website { get; set; } = string.Empty;

    public string SupportEmail { get; set; } = string.Empty;

    public string LicenseType { get; set; } = "Freeware";

    public bool IsInstalled { get; set; } = false;

    public bool HasUpdate { get; set; } = false;

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
