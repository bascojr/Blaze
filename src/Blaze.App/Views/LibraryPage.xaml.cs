using System.Windows;
using System.Windows.Controls;
using Blaze.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for LibraryPage.xaml
/// </summary>
public partial class LibraryPage : Page
{
    private readonly LibraryViewModel _viewModel;
    private bool _showGridView = true;

    public LibraryPage()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<LibraryViewModel>();
        DataContext = _viewModel;

        Loaded += LibraryPage_Loaded;
    }

    private async void LibraryPage_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();

        AppsGridView.ItemsSource = _viewModel.FilteredApps;
        AppsListView.ItemsSource = _viewModel.FilteredApps;

        UpdateEmptyState();
    }

    private void UpdateEmptyState()
    {
        bool isEmpty = !_viewModel.FilteredApps.Any();
        EmptyState.Visibility = isEmpty ? Visibility.Visible : Visibility.Collapsed;
        AppsGridView.Visibility = isEmpty ? Visibility.Collapsed : (_showGridView ? Visibility.Visible : Visibility.Collapsed);
        AppsListView.Visibility = isEmpty ? Visibility.Collapsed : (_showGridView ? Visibility.Collapsed : Visibility.Visible);
    }

    private async void LaunchApp_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string appId)
        {
            var app = _viewModel.FilteredApps.FirstOrDefault(a => a.ApplicationId == appId);
            if (app != null)
            {
                await _viewModel.LaunchAppCommand.ExecuteAsync(app);
            }
        }
    }

    private async void StopApp_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string appId)
        {
            var app = _viewModel.FilteredApps.FirstOrDefault(a => a.ApplicationId == appId);
            if (app != null)
            {
                await _viewModel.StopAppCommand.ExecuteAsync(app);
            }
        }
    }

    private void ShowAppMenu_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            var contextMenu = new ContextMenu
            {
                Style = (Style)FindResource("BlazeContextMenuStyle")
            };

            var uninstallItem = new MenuItem { Header = "Uninstall" };
            uninstallItem.Click += async (s, args) =>
            {
                if (button.Tag is string appId)
                {
                    var app = _viewModel.FilteredApps.FirstOrDefault(a => a.ApplicationId == appId);
                    if (app != null)
                    {
                        await _viewModel.UninstallAppCommand.ExecuteAsync(app);
                    }
                }
            };

            var favoriteItem = new MenuItem { Header = "Toggle Favorite" };
            favoriteItem.Click += async (s, args) =>
            {
                if (button.Tag is string appId)
                {
                    var app = _viewModel.FilteredApps.FirstOrDefault(a => a.ApplicationId == appId);
                    if (app != null)
                    {
                        await _viewModel.ToggleFavoriteCommand.ExecuteAsync(app);
                    }
                }
            };

            contextMenu.Items.Add(favoriteItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(uninstallItem);

            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
        }
    }

    private void ToggleView_Click(object sender, RoutedEventArgs e)
    {
        _showGridView = !_showGridView;
        ViewToggleText.Text = _showGridView ? "List View" : "Grid View";
        UpdateEmptyState();
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SortComboBox.SelectedItem is ComboBoxItem item)
        {
            _viewModel.ChangeSortOrderCommand.Execute(item.Content?.ToString() ?? "Name");
        }
    }

    private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FilterComboBox.SelectedItem is ComboBoxItem item)
        {
            _viewModel.ChangeFilterCommand.Execute(item.Content?.ToString() ?? "All");
        }
    }

    private void BrowseStore_Click(object sender, RoutedEventArgs e)
    {
        NavigationService?.Navigate(new StorePage());
    }
}
