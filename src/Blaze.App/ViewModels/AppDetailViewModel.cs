using System.Collections.ObjectModel;
using Blaze.Core.Models;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the App Detail view
/// </summary>
public partial class AppDetailViewModel : ViewModelBase
{
    private readonly IAppService _appService;
    private readonly IDownloadService _downloadService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private Application? _app;

    [ObservableProperty]
    private InstalledApp? _installedApp;

    [ObservableProperty]
    private bool _isInstalled;

    [ObservableProperty]
    private bool _isDownloading;

    [ObservableProperty]
    private double _downloadProgress;

    [ObservableProperty]
    private string _downloadStatus = string.Empty;

    [ObservableProperty]
    private bool _isInWishlist;

    [ObservableProperty]
    private int _selectedScreenshotIndex;

    public ObservableCollection<Review> Reviews { get; } = new();
    public ObservableCollection<Application> SimilarApps { get; } = new();

    public AppDetailViewModel(
        IAppService appService,
        IDownloadService downloadService,
        ISettingsService settingsService)
    {
        _appService = appService;
        _downloadService = downloadService;
        _settingsService = settingsService;

        _downloadService.DownloadProgressChanged += OnDownloadProgressChanged;
        _downloadService.DownloadCompleted += OnDownloadCompleted;
    }

    public async Task LoadAppAsync(string appId)
    {
        IsLoading = true;

        try
        {
            App = await _appService.GetAppByIdAsync(appId);
            if (App == null) return;

            // Check if installed
            InstalledApp = await _appService.GetInstalledAppAsync(appId);
            IsInstalled = InstalledApp != null;

            // Load reviews
            var reviews = await _appService.GetAppReviewsAsync(appId);
            Reviews.Clear();
            foreach (var review in reviews.Take(10))
            {
                Reviews.Add(review);
            }

            // Load similar apps (same category)
            if (!string.IsNullOrEmpty(App.CategoryId))
            {
                var similar = await _appService.GetAppsByCategoryAsync(App.CategoryId);
                SimilarApps.Clear();
                foreach (var app in similar.Where(a => a.Id != appId).Take(6))
                {
                    SimilarApps.Add(app);
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task Install()
    {
        if (App == null) return;

        var settings = await _settingsService.GetDownloadSettingsAsync();
        var installPath = Path.Combine(settings.DefaultInstallLocation, App.Name);

        IsDownloading = true;
        DownloadStatus = "Starting download...";

        await _downloadService.QueueDownloadAsync(App, installPath);
    }

    [RelayCommand]
    private async Task Uninstall()
    {
        if (App == null) return;

        var result = await _appService.UninstallAppAsync(App.Id);
        if (result)
        {
            IsInstalled = false;
            InstalledApp = null;
        }
    }

    [RelayCommand]
    private async Task Launch()
    {
        if (App == null) return;
        await _appService.LaunchAppAsync(App.Id);
    }

    [RelayCommand]
    private async Task Update()
    {
        if (App == null) return;
        await _appService.UpdateAppAsync(App.Id);
    }

    [RelayCommand]
    private async Task ToggleWishlist()
    {
        if (App == null) return;

        if (IsInWishlist)
        {
            await _appService.RemoveFromWishlistAsync(App.Id);
            IsInWishlist = false;
        }
        else
        {
            await _appService.AddToWishlistAsync(App.Id);
            IsInWishlist = true;
        }
    }

    [RelayCommand]
    private void NextScreenshot()
    {
        if (App?.ScreenshotUrls.Count > 0)
        {
            SelectedScreenshotIndex = (SelectedScreenshotIndex + 1) % App.ScreenshotUrls.Count;
        }
    }

    [RelayCommand]
    private void PreviousScreenshot()
    {
        if (App?.ScreenshotUrls.Count > 0)
        {
            SelectedScreenshotIndex = SelectedScreenshotIndex > 0
                ? SelectedScreenshotIndex - 1
                : App.ScreenshotUrls.Count - 1;
        }
    }

    [RelayCommand]
    private void OpenWebsite()
    {
        if (!string.IsNullOrEmpty(App?.Website))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = App.Website,
                UseShellExecute = true
            });
        }
    }

    private void OnDownloadProgressChanged(object? sender, Core.Events.DownloadProgressEventArgs e)
    {
        if (App != null && e.Download.ApplicationId == App.Id)
        {
            DownloadProgress = e.ProgressPercentage;
            DownloadStatus = $"Downloading... {e.ProgressPercentage:F1}% ({e.Download.SpeedFormatted})";
        }
    }

    private async void OnDownloadCompleted(object? sender, Core.Events.DownloadCompletedEventArgs e)
    {
        if (App != null && e.Download.ApplicationId == App.Id)
        {
            IsDownloading = false;

            if (e.Success)
            {
                // Register as installed
                await _appService.InstallAppAsync(App, e.Download.DestinationPath);
                IsInstalled = true;
                InstalledApp = await _appService.GetInstalledAppAsync(App.Id);
                DownloadStatus = "Installation complete!";
            }
            else
            {
                DownloadStatus = $"Download failed: {e.ErrorMessage}";
            }
        }
    }

    public override void Cleanup()
    {
        _downloadService.DownloadProgressChanged -= OnDownloadProgressChanged;
        _downloadService.DownloadCompleted -= OnDownloadCompleted;
    }
}
