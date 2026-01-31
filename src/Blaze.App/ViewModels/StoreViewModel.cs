using System.Collections.ObjectModel;
using System.IO;
using Blaze.Core.Models;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the Store view
/// </summary>
public partial class StoreViewModel : ViewModelBase
{
    private readonly IAppService _appService;
    private readonly IDownloadService _downloadService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private Category? _selectedCategory;

    [ObservableProperty]
    private Application? _selectedApp;

    [ObservableProperty]
    private string _sortBy = "Popular";

    public ObservableCollection<Application> FeaturedApps { get; } = new();
    public ObservableCollection<Application> PopularApps { get; } = new();
    public ObservableCollection<Application> NewReleases { get; } = new();
    public ObservableCollection<Application> AllApps { get; } = new();
    public ObservableCollection<Application> SearchResults { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();

    public string[] SortOptions { get; } = { "Popular", "New Releases", "Name A-Z", "Name Z-A", "Rating" };

    public StoreViewModel(IAppService appService, IDownloadService downloadService, ISettingsService settingsService)
    {
        _appService = appService;
        _downloadService = downloadService;
        _settingsService = settingsService;
    }

    public override async Task InitializeAsync()
    {
        IsLoading = true;

        try
        {
            // Load categories
            var categories = await _appService.GetCategoriesAsync();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            // Load featured apps
            var featured = await _appService.GetFeaturedAppsAsync();
            FeaturedApps.Clear();
            foreach (var app in featured)
            {
                FeaturedApps.Add(app);
            }

            // Load popular apps
            var popular = await _appService.GetPopularAppsAsync(12);
            PopularApps.Clear();
            foreach (var app in popular)
            {
                PopularApps.Add(app);
            }

            // Load new releases
            var newReleases = await _appService.GetNewReleasesAsync(12);
            NewReleases.Clear();
            foreach (var app in newReleases)
            {
                NewReleases.Add(app);
            }

            // Load all apps
            await LoadAllApps();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadAllApps()
    {
        IEnumerable<Application> apps;

        if (SelectedCategory != null)
        {
            apps = await _appService.GetAppsByCategoryAsync(SelectedCategory.Id);
        }
        else
        {
            apps = await _appService.GetAllAppsAsync();
        }

        // Apply sorting
        apps = SortBy switch
        {
            "Popular" => apps.OrderByDescending(a => a.DownloadCount),
            "New Releases" => apps.OrderByDescending(a => a.ReleaseDate),
            "Name A-Z" => apps.OrderBy(a => a.Name),
            "Name Z-A" => apps.OrderByDescending(a => a.Name),
            "Rating" => apps.OrderByDescending(a => a.Rating),
            _ => apps
        };

        AllApps.Clear();
        foreach (var app in apps)
        {
            AllApps.Add(app);
        }
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            SearchResults.Clear();
            return;
        }

        IsLoading = true;

        try
        {
            var results = await _appService.SearchAppsAsync(SearchQuery);
            SearchResults.Clear();
            foreach (var app in results)
            {
                SearchResults.Add(app);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SelectCategory(Category? category)
    {
        SelectedCategory = category;
        await LoadAllApps();
    }

    [RelayCommand]
    private void SelectApp(Application app)
    {
        SelectedApp = app;
    }

    [RelayCommand]
    private async Task InstallApp(Application app)
    {
        if (app == null) return;

        var settings = await _settingsService.GetDownloadSettingsAsync();
        var installPath = Path.Combine(settings.DefaultInstallLocation, app.Name);

        await _downloadService.QueueDownloadAsync(app, installPath);
    }

    [RelayCommand]
    private async Task AddToWishlist(Application app)
    {
        if (app == null) return;
        await _appService.AddToWishlistAsync(app.Id);
    }

    [RelayCommand]
    private async Task ChangeSortOrder(string sortBy)
    {
        SortBy = sortBy;
        await LoadAllApps();
    }

    partial void OnSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SearchResults.Clear();
        }
    }
}
