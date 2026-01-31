using System.Windows;
using System.Windows.Controls;
using Blaze.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for DownloadsPage.xaml
/// </summary>
public partial class DownloadsPage : Page
{
    private readonly DownloadsViewModel _viewModel;

    public DownloadsPage()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<DownloadsViewModel>();
        DataContext = _viewModel;

        Loaded += DownloadsPage_Loaded;
    }

    private async void DownloadsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();

        ActiveDownloadsList.ItemsSource = _viewModel.ActiveDownloads;
        QueuedDownloadsList.ItemsSource = _viewModel.QueuedDownloads;
        CompletedDownloadsList.ItemsSource = _viewModel.CompletedDownloads;

        UpdateUI();

        // Subscribe to changes
        _viewModel.PropertyChanged += (s, args) =>
        {
            if (args.PropertyName == nameof(_viewModel.TotalSpeedFormatted))
            {
                SpeedText.Text = _viewModel.TotalSpeedFormatted;
            }
            else if (args.PropertyName == nameof(_viewModel.IsPaused))
            {
                PauseResumeButton.Content = _viewModel.IsPaused ? "Resume All" : "Pause All";
            }
        };
    }

    private void UpdateUI()
    {
        SpeedText.Text = _viewModel.TotalSpeedFormatted;
        PauseResumeButton.Content = _viewModel.IsPaused ? "Resume All" : "Pause All";
        EmptyActiveState.Visibility = _viewModel.ActiveDownloads.Count == 0 && _viewModel.QueuedDownloads.Count == 0
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private async void PauseDownload_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string downloadId)
        {
            var download = _viewModel.ActiveDownloads.FirstOrDefault(d => d.Id == downloadId);
            if (download != null)
            {
                await _viewModel.PauseDownloadCommand.ExecuteAsync(download);
            }
        }
    }

    private async void ResumeDownload_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string downloadId)
        {
            var download = _viewModel.QueuedDownloads.FirstOrDefault(d => d.Id == downloadId)
                          ?? _viewModel.ActiveDownloads.FirstOrDefault(d => d.Id == downloadId);
            if (download != null)
            {
                await _viewModel.ResumeDownloadCommand.ExecuteAsync(download);
            }
        }
    }

    private async void CancelDownload_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string downloadId)
        {
            var download = _viewModel.ActiveDownloads.FirstOrDefault(d => d.Id == downloadId)
                          ?? _viewModel.QueuedDownloads.FirstOrDefault(d => d.Id == downloadId);
            if (download != null)
            {
                await _viewModel.CancelDownloadCommand.ExecuteAsync(download);
            }
        }
    }

    private async void PauseResumeAll_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.IsPaused)
        {
            await _viewModel.ResumeAllCommand.ExecuteAsync(null);
        }
        else
        {
            await _viewModel.PauseAllCommand.ExecuteAsync(null);
        }
    }

    private async void ClearCompleted_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.ClearCompletedCommand.ExecuteAsync(null);
    }
}
