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
        if (_viewModel.IsDownloading)
        {
            MainActionButton.Visibility = Visibility.Collapsed;
            DownloadProgressPanel.Visibility = Visibility.Visible;
        }
        else
        {
            MainActionButton.Visibility = Visibility.Visible;
            DownloadProgressPanel.Visibility = Visibility.Collapsed;

            if (_viewModel.IsInstalled)
            {
                MainActionButton.Content = "Launch";
                MainActionButton.Style = (Style)FindResource("PrimaryButtonStyle");
            }
            else
            {
                MainActionButton.Content = "Install";
                MainActionButton.Style = (Style)FindResource("InstallButtonStyle");
            }
        }

        WishlistButton.Content = _viewModel.IsInWishlist ? "In Wishlist" : "Add to Wishlist";
    }

    private async void MainAction_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.IsInstalled)
        {
            await _viewModel.LaunchCommand.ExecuteAsync(null);
        }
        else
        {
            await _viewModel.InstallCommand.ExecuteAsync(null);
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
