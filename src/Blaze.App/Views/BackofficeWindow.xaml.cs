using System.Windows;
using Blaze.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for BackofficeWindow.xaml
/// </summary>
public partial class BackofficeWindow : Window
{
    private readonly BackofficeViewModel _viewModel;

    public BackofficeWindow()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<BackofficeViewModel>();
        DataContext = _viewModel;

        _viewModel.CloseRequested += OnCloseRequested;
        Loaded += BackofficeWindow_Loaded;
    }

    private async void BackofficeWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.InitializeAsync();
    }

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.CloseRequested -= OnCloseRequested;
        _viewModel.Cleanup();
        base.OnClosed(e);
    }
}
