using System.Windows;
using System.Windows.Controls;
using Blaze.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : Page
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<SettingsViewModel>();
        DataContext = _viewModel;

        Loaded += SettingsPage_Loaded;
    }

    private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();
    }

    private void BrowseFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select Default Install Location"
        };

        if (dialog.ShowDialog() == true)
        {
            _viewModel.DefaultInstallLocation = dialog.FolderName;
        }
    }

    private async void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.SaveSettingsCommand.ExecuteAsync(null);
        MessageBox.Show("Settings saved successfully!", "Blaze", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private async void ResetDefaults_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Are you sure you want to reset all settings to defaults?",
            "Reset Settings",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            await _viewModel.ResetToDefaultsCommand.ExecuteAsync(null);
        }
    }

    private void OpenDataFolder_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.OpenDataFolderCommand.Execute(null);
    }

    private async void ClearCache_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.ClearCacheCommand.ExecuteAsync(null);
        MessageBox.Show("Cache cleared successfully!", "Blaze", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
