using System.Collections.ObjectModel;
using Blaze.Core.Models;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the main window
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly IAppService _appService;
    private readonly IDownloadService _downloadService;
    private readonly IAuthService _authService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _currentView = "Store";

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private bool _isLoggedIn;

    [ObservableProperty]
    private int _activeDownloadsCount;

    [ObservableProperty]
    private int _installedAppsCount;

    [ObservableProperty]
    private int _updatesAvailableCount;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public ObservableCollection<Download> ActiveDownloads { get; } = new();

    public MainViewModel(
        IAppService appService,
        IDownloadService downloadService,
        IAuthService authService,
        ISettingsService settingsService)
    {
        _appService = appService;
        _downloadService = downloadService;
        _authService = authService;
        _settingsService = settingsService;

        _authService.UserLoggedIn += OnUserLoggedIn;
        _authService.UserLoggedOut += OnUserLoggedOut;
        _downloadService.DownloadProgressChanged += OnDownloadProgressChanged;
        _downloadService.DownloadCompleted += OnDownloadCompleted;
    }

    public override async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            // Check authentication status
            if (_authService.IsAuthenticated)
            {
                CurrentUser = _authService.CurrentUser;
                IsLoggedIn = true;
            }

            // Load counts
            var installedApps = await _appService.GetInstalledAppsAsync();
            InstalledAppsCount = installedApps.Count();

            var updates = await _appService.CheckForUpdatesAsync();
            UpdatesAvailableCount = updates.Count();

            var downloads = await _downloadService.GetActiveDownloadsAsync();
            ActiveDownloadsCount = downloads.Count();

            foreach (var download in downloads)
            {
                ActiveDownloads.Add(download);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void NavigateTo(string view)
    {
        CurrentView = view;
    }

    [RelayCommand]
    private async Task Login(string username)
    {
        if (string.IsNullOrWhiteSpace(username)) return;

        var result = await _authService.LoginAsync(username, "password");
        if (result.Success)
        {
            CurrentUser = result.User;
            IsLoggedIn = true;
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        await _authService.LogoutAsync();
        CurrentUser = null;
        IsLoggedIn = false;
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) return;

        CurrentView = "Store";
        // Search will be handled by StoreViewModel
    }

    [RelayCommand]
    private async Task RefreshDownloads()
    {
        ActiveDownloads.Clear();
        var downloads = await _downloadService.GetActiveDownloadsAsync();
        foreach (var download in downloads)
        {
            ActiveDownloads.Add(download);
        }
        ActiveDownloadsCount = ActiveDownloads.Count;
    }

    private void OnUserLoggedIn(object? sender, User user)
    {
        CurrentUser = user;
        IsLoggedIn = true;
    }

    private void OnUserLoggedOut(object? sender, EventArgs e)
    {
        CurrentUser = null;
        IsLoggedIn = false;
    }

    private void OnDownloadProgressChanged(object? sender, Core.Events.DownloadProgressEventArgs e)
    {
        var existing = ActiveDownloads.FirstOrDefault(d => d.Id == e.Download.Id);
        if (existing != null)
        {
            existing.DownloadedBytes = e.BytesDownloaded;
            existing.SpeedBytesPerSecond = e.SpeedBytesPerSecond;
        }
    }

    private void OnDownloadCompleted(object? sender, Core.Events.DownloadCompletedEventArgs e)
    {
        var existing = ActiveDownloads.FirstOrDefault(d => d.Id == e.Download.Id);
        if (existing != null)
        {
            ActiveDownloads.Remove(existing);
            ActiveDownloadsCount = ActiveDownloads.Count;
        }
    }

    public override void Cleanup()
    {
        _authService.UserLoggedIn -= OnUserLoggedIn;
        _authService.UserLoggedOut -= OnUserLoggedOut;
        _downloadService.DownloadProgressChanged -= OnDownloadProgressChanged;
        _downloadService.DownloadCompleted -= OnDownloadCompleted;
    }
}
