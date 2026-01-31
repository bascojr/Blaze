using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Blaze.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for StorePage.xaml
/// </summary>
public partial class StorePage : Page
{
    private readonly StoreViewModel _viewModel;

    public StorePage()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<StoreViewModel>();
        DataContext = _viewModel;

        Loaded += StorePage_Loaded;
    }

    private async void StorePage_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();

        // Bind the ItemsControls
        FeaturedApps.ItemsSource = _viewModel.FeaturedApps;
        PopularApps.ItemsSource = _viewModel.PopularApps;
        AllApps.ItemsSource = _viewModel.AllApps;
    }

    private void AppCard_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.Tag is string appId)
        {
            NavigationService?.Navigate(new AppDetailPage(appId));
        }
    }

    private async void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SortComboBox.SelectedItem is ComboBoxItem item)
        {
            await _viewModel.ChangeSortOrderCommand.ExecuteAsync(item.Content?.ToString() ?? "Popular");
        }
    }
}
