using Blaze.Core.Models;

namespace Blaze.Core.Events;

/// <summary>
/// Event arguments for download progress updates
/// </summary>
public class DownloadProgressEventArgs : EventArgs
{
    public Download Download { get; }
    public long BytesDownloaded { get; }
    public long TotalBytes { get; }
    public double SpeedBytesPerSecond { get; }
    public double ProgressPercentage => TotalBytes > 0 ? (double)BytesDownloaded / TotalBytes * 100 : 0;

    public DownloadProgressEventArgs(Download download, long bytesDownloaded, long totalBytes, double speed)
    {
        Download = download;
        BytesDownloaded = bytesDownloaded;
        TotalBytes = totalBytes;
        SpeedBytesPerSecond = speed;
    }
}

/// <summary>
/// Event arguments for download completion
/// </summary>
public class DownloadCompletedEventArgs : EventArgs
{
    public Download Download { get; }
    public bool Success { get; }
    public string? ErrorMessage { get; }

    public DownloadCompletedEventArgs(Download download, bool success, string? errorMessage = null)
    {
        Download = download;
        Success = success;
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Event arguments for app launch events
/// </summary>
public class AppLaunchEventArgs : EventArgs
{
    public InstalledApp App { get; }
    public int ProcessId { get; }

    public AppLaunchEventArgs(InstalledApp app, int processId)
    {
        App = app;
        ProcessId = processId;
    }
}

/// <summary>
/// Event arguments for app exit events
/// </summary>
public class AppExitEventArgs : EventArgs
{
    public InstalledApp App { get; }
    public TimeSpan SessionDuration { get; }

    public AppExitEventArgs(InstalledApp app, TimeSpan sessionDuration)
    {
        App = app;
        SessionDuration = sessionDuration;
    }
}

/// <summary>
/// Event arguments for installation progress
/// </summary>
public class InstallProgressEventArgs : EventArgs
{
    public string ApplicationId { get; }
    public string ApplicationName { get; }
    public InstallStage Stage { get; }
    public double Progress { get; }
    public string StatusMessage { get; }

    public InstallProgressEventArgs(string appId, string appName, InstallStage stage, double progress, string message)
    {
        ApplicationId = appId;
        ApplicationName = appName;
        Stage = stage;
        Progress = progress;
        StatusMessage = message;
    }
}

public enum InstallStage
{
    Preparing,
    Extracting,
    Installing,
    ConfiguringShortcuts,
    RegisteringApp,
    Completing,
    Completed,
    Failed
}

/// <summary>
/// Event arguments for update availability
/// </summary>
public class UpdateAvailableEventArgs : EventArgs
{
    public AppUpdate Update { get; }

    public UpdateAvailableEventArgs(AppUpdate update)
    {
        Update = update;
    }
}
