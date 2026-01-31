using System.Collections.ObjectModel;
using Blaze.Core.Events;
using Blaze.Core.Models;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the Downloads view
/// </summary>
public partial class DownloadsViewModel : ViewModelBase
{
    private readonly IDownloadService _downloadService;

    [ObservableProperty]
    private bool _isPaused;

    [ObservableProperty]
    private double _totalSpeed;

    [ObservableProperty]
    private string _totalSpeedFormatted = "0 B/s";

    [ObservableProperty]
    private int _maxConcurrentDownloads = 3;

    [ObservableProperty]
    private int _speedLimitMbps = 0;

    public ObservableCollection<Download> ActiveDownloads { get; } = new();
    public ObservableCollection<Download> QueuedDownloads { get; } = new();
    public ObservableCollection<Download> CompletedDownloads { get; } = new();

    public DownloadsViewModel(IDownloadService downloadService)
    {
        _downloadService = downloadService;

        _downloadService.DownloadProgressChanged += OnDownloadProgressChanged;
        _downloadService.DownloadCompleted += OnDownloadCompleted;
    }

    public override async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            await RefreshDownloads();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task RefreshDownloads()
    {
        var queue = await _downloadService.GetQueueAsync();

        ActiveDownloads.Clear();
        foreach (var download in queue.ActiveDownloads)
        {
            ActiveDownloads.Add(download);
        }

        QueuedDownloads.Clear();
        foreach (var download in queue.QueuedDownloads)
        {
            QueuedDownloads.Add(download);
        }

        var completed = await _downloadService.GetCompletedDownloadsAsync();
        CompletedDownloads.Clear();
        foreach (var download in completed)
        {
            CompletedDownloads.Add(download);
        }

        UpdateTotalSpeed();
    }

    private void UpdateTotalSpeed()
    {
        TotalSpeed = ActiveDownloads.Sum(d => d.SpeedBytesPerSecond);
        TotalSpeedFormatted = FormatSpeed(TotalSpeed);
    }

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

    [RelayCommand]
    private async Task PauseDownload(Download download)
    {
        if (download == null) return;
        await _downloadService.PauseDownloadAsync(download.Id);
        await RefreshDownloads();
    }

    [RelayCommand]
    private async Task ResumeDownload(Download download)
    {
        if (download == null) return;
        await _downloadService.ResumeDownloadAsync(download.Id);
        await RefreshDownloads();
    }

    [RelayCommand]
    private async Task CancelDownload(Download download)
    {
        if (download == null) return;
        await _downloadService.CancelDownloadAsync(download.Id);

        ActiveDownloads.Remove(download);
        QueuedDownloads.Remove(download);
    }

    [RelayCommand]
    private async Task RetryDownload(Download download)
    {
        if (download == null) return;
        await _downloadService.RetryDownloadAsync(download.Id);
        await RefreshDownloads();
    }

    [RelayCommand]
    private async Task PauseAll()
    {
        await _downloadService.PauseAllDownloadsAsync();
        IsPaused = true;
        await RefreshDownloads();
    }

    [RelayCommand]
    private async Task ResumeAll()
    {
        await _downloadService.ResumeAllDownloadsAsync();
        IsPaused = false;
        await RefreshDownloads();
    }

    [RelayCommand]
    private async Task ClearCompleted()
    {
        await _downloadService.ClearCompletedDownloadsAsync();
        CompletedDownloads.Clear();
    }

    [RelayCommand]
    private async Task SetMaxConcurrent(int count)
    {
        MaxConcurrentDownloads = count;
        await _downloadService.SetMaxConcurrentDownloadsAsync(count);
    }

    [RelayCommand]
    private async Task SetSpeedLimit(int mbps)
    {
        SpeedLimitMbps = mbps;
        await _downloadService.SetSpeedLimitAsync(mbps);
    }

    private void OnDownloadProgressChanged(object? sender, DownloadProgressEventArgs e)
    {
        var download = ActiveDownloads.FirstOrDefault(d => d.Id == e.Download.Id);
        if (download != null)
        {
            download.DownloadedBytes = e.BytesDownloaded;
            download.SpeedBytesPerSecond = e.SpeedBytesPerSecond;
        }
        UpdateTotalSpeed();
    }

    private async void OnDownloadCompleted(object? sender, DownloadCompletedEventArgs e)
    {
        await RefreshDownloads();
    }

    public override void Cleanup()
    {
        _downloadService.DownloadProgressChanged -= OnDownloadProgressChanged;
        _downloadService.DownloadCompleted -= OnDownloadCompleted;
    }
}
