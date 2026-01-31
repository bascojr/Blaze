using Blaze.Core.Events;
using Blaze.Core.Models;

namespace Blaze.Core.Services;

/// <summary>
/// Service interface for managing downloads
/// </summary>
public interface IDownloadService
{
    // Events
    event EventHandler<DownloadProgressEventArgs>? DownloadProgressChanged;
    event EventHandler<DownloadCompletedEventArgs>? DownloadCompleted;
    event EventHandler<InstallProgressEventArgs>? InstallProgressChanged;

    // Queue management
    Task<Download> QueueDownloadAsync(Application app, string destinationPath);
    Task<bool> StartDownloadAsync(string downloadId);
    Task<bool> PauseDownloadAsync(string downloadId);
    Task<bool> ResumeDownloadAsync(string downloadId);
    Task<bool> CancelDownloadAsync(string downloadId);
    Task<bool> RetryDownloadAsync(string downloadId);

    // Queue operations
    Task<DownloadQueue> GetQueueAsync();
    Task<Download?> GetDownloadAsync(string downloadId);
    Task<IEnumerable<Download>> GetActiveDownloadsAsync();
    Task<IEnumerable<Download>> GetCompletedDownloadsAsync();
    Task ClearCompletedDownloadsAsync();

    // Queue control
    Task PauseAllDownloadsAsync();
    Task ResumeAllDownloadsAsync();
    Task SetMaxConcurrentDownloadsAsync(int count);
    Task SetSpeedLimitAsync(int mbps);

    // Statistics
    Task<long> GetTotalDownloadedBytesAsync();
    Task<double> GetCurrentSpeedAsync();
}
