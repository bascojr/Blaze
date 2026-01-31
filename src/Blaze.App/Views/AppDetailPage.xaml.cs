using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Blaze.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for AppDetailPage.xaml
/// </summary>
public partial class AppDetailPage : Page
{
    private readonly AppDetailViewModel _viewModel;
    private readonly string _appId;

    public AppDetailPage(string appId)
    {
        InitializeComponent();

        _appId = appId;
        _viewModel = App.Services.GetRequiredService<AppDetailViewModel>();
        DataContext = _viewModel;

        Loaded += AppDetailPage_Loaded;

        // Subscribe to progress updates
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private async void AppDetailPage_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadAppAsync(_appId);

        SimilarAppsList.ItemsSource = _viewModel.SimilarApps;

        UpdateUI();
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            switch (e.PropertyName)
            {
                case nameof(_viewModel.IsInstalled):
                case nameof(_viewModel.IsDownloading):
                case nameof(_viewModel.HasUpdate):
                case nameof(_viewModel.IsUpdating):
                    UpdateUI();
                    break;
                case nameof(_viewModel.DownloadProgress):
                    DownloadProgressBar.Width = _viewModel.DownloadProgress * 2; // 200px max width
                    break;
                case nameof(_viewModel.DownloadStatus):
                    DownloadStatusText.Text = _viewModel.DownloadStatus;
                    break;
            }
        });
    }

    private void UpdateUI()
    {
        // Handle downloading state
        if (_viewModel.IsDownloading)
        {
            InstallButton.Visibility = Visibility.Collapsed;
            LaunchButton.Visibility = Visibility.Collapsed;
            UpdateButton.Visibility = Visibility.Collapsed;
            DownloadProgressPanel.Visibility = Visibility.Visible;
            LaunchSmallButton.Visibility = Visibility.Collapsed;
            UninstallButton.Visibility = Visibility.Collapsed;
        }
        else
        {
            DownloadProgressPanel.Visibility = Visibility.Collapsed;

            if (_viewModel.IsInstalled)
            {
                // App is installed
                InstallButton.Visibility = Visibility.Collapsed;
                VersionInfoPanel.Visibility = Visibility.Visible;
                InstalledVersionText.Text = $"Installed: v{_viewModel.InstalledVersion}";
                UninstallButton.Visibility = Visibility.Visible;

                if (_viewModel.HasUpdate)
                {
                    // Update available
                    LaunchButton.Visibility = Visibility.Collapsed;
                    UpdateButton.Visibility = Visibility.Visible;
                    UpdateAvailableText.Visibility = Visibility.Visible;
                    UpdateAvailableText.Text = $"Update available: v{_viewModel.App?.Version}";
                    LaunchSmallButton.Visibility = Visibility.Visible;
                }
                else
                {
                    // No update, show launch as main button
                    LaunchButton.Visibility = Visibility.Visible;
                    UpdateButton.Visibility = Visibility.Collapsed;
                    UpdateAvailableText.Visibility = Visibility.Collapsed;
                    LaunchSmallButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                // App not installed
                InstallButton.Visibility = Visibility.Visible;
                LaunchButton.Visibility = Visibility.Collapsed;
                UpdateButton.Visibility = Visibility.Collapsed;
                VersionInfoPanel.Visibility = Visibility.Collapsed;
                LaunchSmallButton.Visibility = Visibility.Collapsed;
                UninstallButton.Visibility = Visibility.Collapsed;
            }
        }

        WishlistButton.Content = _viewModel.IsInWishlist ? "In Wishlist" : "Add to Wishlist";
    }

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.InstallCommand.ExecuteAsync(null);
    }

    private async void Launch_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.LaunchCommand.ExecuteAsync(null);
    }

    private async void Update_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.UpdateCommand.ExecuteAsync(null);
    }

    private async void Uninstall_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            $"Are you sure you want to uninstall {_viewModel.App?.Name}?",
            "Uninstall Application",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            await _viewModel.UninstallCommand.ExecuteAsync(null);
            UpdateUI();
        }
    }

    private async void ToggleWishlist_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.ToggleWishlistCommand.ExecuteAsync(null);
        UpdateUI();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService?.CanGoBack == true)
        {
            NavigationService.GoBack();
        }
        else
        {
            NavigationService?.Navigate(new StorePage());
        }
    }

    private void Website_Click(object sender, MouseButtonEventArgs e)
    {
        _viewModel.OpenWebsiteCommand.Execute(null);
    }

    private void SimilarApp_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.Tag is string appId)
        {
            NavigationService?.Navigate(new AppDetailPage(appId));
        }
    }
}
