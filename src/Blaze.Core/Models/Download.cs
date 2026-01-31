using System.ComponentModel.DataAnnotations;

namespace Blaze.Core.Models;

/// <summary>
/// Represents a download in progress or completed
/// </summary>
public class Download
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string ApplicationId { get; set; } = string.Empty;

    public Application? Application { get; set; }

    public string ApplicationName { get; set; } = string.Empty;

    public DownloadStatus Status { get; set; } = DownloadStatus.Queued;

    public long TotalBytes { get; set; }

    public long DownloadedBytes { get; set; }

    public double Progress => TotalBytes > 0 ? (double)DownloadedBytes / TotalBytes * 100 : 0;

    public string ProgressFormatted => $"{Progress:F1}%";

    public double SpeedBytesPerSecond { get; set; }

    public string SpeedFormatted => FormatSpeed(SpeedBytesPerSecond);

    public TimeSpan EstimatedTimeRemaining
    {
        get
        {
            if (SpeedBytesPerSecond <= 0) return TimeSpan.MaxValue;
            var remainingBytes = TotalBytes - DownloadedBytes;
            var seconds = remainingBytes / SpeedBytesPerSecond;
            return TimeSpan.FromSeconds(seconds);
        }
    }

    public string TimeRemainingFormatted
    {
        get
        {
            var time = EstimatedTimeRemaining;
            if (time == TimeSpan.MaxValue) return "Calculating...";
            if (time.TotalHours >= 1) return $"{time.Hours}h {time.Minutes}m";
            if (time.TotalMinutes >= 1) return $"{time.Minutes}m {time.Seconds}s";
            return $"{time.Seconds}s";
        }
    }

    public DateTime StartedAt { get; set; } = DateTime.Now;

    public DateTime? CompletedAt { get; set; }

    public string DestinationPath { get; set; } = string.Empty;

    public string TempFilePath { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public int RetryCount { get; set; } = 0;

    public int MaxRetries { get; set; } = 3;

    public bool IsPaused => Status == DownloadStatus.Paused;

    public bool IsActive => Status == DownloadStatus.Downloading;

    public bool IsCompleted => Status == DownloadStatus.Completed;

    public bool IsFailed => Status == DownloadStatus.Failed;

    private static string FormatSpeed(double bytesPerSecond)
    {
        string[] sizes = { "B/s", "KB/s", "MB/s", "GB/s" };
        int order = 0;
        double speed = bytesPerSecond;
        while (speed >= 1024 && order < sizes.Length - 1)
        {
            order++;
            speed /= 1024;
        }
        return $"{speed:0.##} {sizes[order]}";
    }
}

public enum DownloadStatus
{
    Queued,
    Downloading,
    Paused,
    Installing,
    Completed,
    Failed,
    Cancelled
}

/// <summary>
/// Represents the download queue
/// </summary>
public class DownloadQueue
{
    public List<Download> ActiveDownloads { get; set; } = new();

    public List<Download> QueuedDownloads { get; set; } = new();

    public List<Download> CompletedDownloads { get; set; } = new();

    public int MaxConcurrent { get; set; } = 3;

    public bool IsPaused { get; set; } = false;

    public long TotalBytesDownloaded { get; set; }

    public double AverageSpeed { get; set; }
}
