using System.Windows;
using System.Windows.Input;
using Blaze.App.ViewModels;
using Blaze.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<MainViewModel>();
        DataContext = _viewModel;

        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();

        // Navigate to Store by default
        NavigateToStore_Click(sender, e);
    }

    private void NavigateToStore_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new StorePage());
        _viewModel.CurrentView = "Store";
    }

    private void NavigateToLibrary_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new LibraryPage());
        _viewModel.CurrentView = "Library";
    }

    private void NavigateToDownloads_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new DownloadsPage());
        _viewModel.CurrentView = "Downloads";
    }

    private void NavigateToSettings_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new SettingsPage());
        _viewModel.CurrentView = "Settings";
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _viewModel.SearchCommand.Execute(null);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.Cleanup();
        base.OnClosed(e);
    }
}
