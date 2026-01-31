using System.Windows;
using System.Windows.Input;
using Blaze.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Blaze.App.Views;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public bool IsLoginSuccessful => _viewModel.IsLoginSuccessful;

    public LoginWindow()
    {
        InitializeComponent();

        _viewModel = App.Services.GetRequiredService<LoginViewModel>();
        DataContext = _viewModel;

        _viewModel.LoginSucceeded += OnLoginSucceeded;
        _viewModel.CloseRequested += OnCloseRequested;

        // Focus the username field when loaded
        Loaded += (s, e) => LoginUsernameBox.Focus();
    }

    private void OnLoginSucceeded(object? sender, EventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void SwitchToRegister_Click(object sender, MouseButtonEventArgs e)
    {
        _viewModel.SwitchToRegisterCommand.Execute(null);
        RegisterUsernameBox.Focus();
    }

    private void SwitchToLogin_Click(object sender, MouseButtonEventArgs e)
    {
        _viewModel.SwitchToLoginCommand.Execute(null);
        LoginUsernameBox.Focus();
    }

    // Password binding workaround (PasswordBox doesn't support binding for security)
    private void LoginPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = LoginPasswordBox.Password;
    }

    private void RegisterPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = RegisterPasswordBox.Password;
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
    }

    protected override void OnClosed(EventArgs e)
    {
        _viewModel.LoginSucceeded -= OnLoginSucceeded;
        _viewModel.CloseRequested -= OnCloseRequested;
        base.OnClosed(e);
    }
}
