using System.Collections.ObjectModel;
using Blaze.Core.Models;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the Library view
/// </summary>
public partial class LibraryViewModel : ViewModelBase
{
    private readonly IAppService _appService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private InstalledApp? _selectedApp;

    [ObservableProperty]
    private string _sortBy = "Name";

    [ObservableProperty]
    private string _filterBy = "All";

    [ObservableProperty]
    private bool _showGridView = true;

    public ObservableCollection<InstalledApp> InstalledApps { get; } = new();
    public ObservableCollection<InstalledApp> FilteredApps { get; } = new();
    public ObservableCollection<InstalledApp> RecentApps { get; } = new();
    public ObservableCollection<InstalledApp> FavoriteApps { get; } = new();

    public string[] SortOptions { get; } = { "Name", "Recently Played", "Install Date", "Size", "Play Time" };
    public string[] FilterOptions { get; } = { "All", "Installed", "Has Updates", "Favorites" };

    public LibraryViewModel(IAppService appService, ISettingsService settingsService)
    {
        _appService = appService;
        _settingsService = settingsService;
    }

    public override async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            await LoadInstalledApps();

            // Load recent apps
            var recent = InstalledApps
                .Where(a => a.LastLaunchedAt != default)
                .OrderByDescending(a => a.LastLaunchedAt)
                .Take(5);

            RecentApps.Clear();
            foreach (var app in recent)
            {
                RecentApps.Add(app);
            }

            // Load favorites
            var favorites = await _appService.GetFavoriteAppsAsync();
            FavoriteApps.Clear();
            foreach (var app in favorites)
            {
                FavoriteApps.Add(app);
            }

            ApplyFilter();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadInstalledApps()
    {
        var apps = await _appService.GetInstalledAppsAsync();

        InstalledApps.Clear();
        foreach (var app in apps)
        {
            InstalledApps.Add(app);
        }
    }

    private void ApplyFilter()
    {
        IEnumerable<InstalledApp> apps = InstalledApps;

        // Apply text search
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.ToLowerInvariant();
            apps = apps.Where(a => a.Name.ToLowerInvariant().Contains(query));
        }

        // Apply filter
        apps = FilterBy switch
        {
            "Has Updates" => apps.Where(a => a.HasUpdate),
            "Favorites" => FavoriteApps,
            _ => apps
        };

        // Apply sorting
        apps = SortBy switch
        {
            "Name" => apps.OrderBy(a => a.Name),
            "Recently Played" => apps.OrderByDescending(a => a.LastLaunchedAt),
            "Install Date" => apps.OrderByDescending(a => a.InstalledAt),
            "Size" => apps.OrderByDescending(a => a.InstalledSizeBytes),
            "Play Time" => apps.OrderByDescending(a => a.TotalPlayTime),
            _ => apps
        };

        FilteredApps.Clear();
        foreach (var app in apps)
        {
            FilteredApps.Add(app);
        }
    }

    [RelayCommand]
    private async Task LaunchApp(InstalledApp app)
    {
        if (app == null) return;

        await _appService.LaunchAppAsync(app.ApplicationId);
        app.IsRunning = true;
        OnPropertyChanged(nameof(FilteredApps));
    }

    [RelayCommand]
    private async Task StopApp(InstalledApp app)
    {
        if (app == null) return;

        await _appService.StopAppAsync(app.ApplicationId);
        app.IsRunning = false;
        OnPropertyChanged(nameof(FilteredApps));
    }

    [RelayCommand]
    private async Task UninstallApp(InstalledApp app)
    {
        if (app == null) return;

        var result = await _appService.UninstallAppAsync(app.ApplicationId);
        if (result)
        {
            InstalledApps.Remove(app);
            FilteredApps.Remove(app);
        }
    }

    [RelayCommand]
    private async Task UpdateApp(InstalledApp app)
    {
        if (app == null || !app.HasUpdate) return;

        await _appService.UpdateAppAsync(app.ApplicationId);
        app.InstalledVersion = app.LatestVersion;
        OnPropertyChanged(nameof(FilteredApps));
    }

    [RelayCommand]
    private async Task ToggleFavorite(InstalledApp app)
    {
        if (app == null) return;

        if (FavoriteApps.Contains(app))
        {
            await _appService.RemoveFromFavoritesAsync(app.ApplicationId);
            FavoriteApps.Remove(app);
        }
        else
        {
            await _appService.AddToFavoritesAsync(app.ApplicationId);
            FavoriteApps.Add(app);
        }
    }

    [RelayCommand]
    private void ToggleView()
    {
        ShowGridView = !ShowGridView;
    }

    [RelayCommand]
    private void ChangeSortOrder(string sortBy)
    {
        SortBy = sortBy;
        ApplyFilter();
    }

    [RelayCommand]
    private void ChangeFilter(string filterBy)
    {
        FilterBy = filterBy;
        ApplyFilter();
    }

    [RelayCommand]
    private void Search()
    {
        ApplyFilter();
    }

    partial void OnSearchQueryChanged(string value)
    {
        ApplyFilter();
    }

    partial void OnSelectedAppChanged(InstalledApp? value)
    {
        // Could trigger detail view
    }
}
