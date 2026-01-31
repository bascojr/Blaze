using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the Login/Register window
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _authService;

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _isRegisterMode;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _hasError;

    [ObservableProperty]
    private bool _rememberMe = true;

    public bool IsLoginSuccessful { get; private set; }

    public event EventHandler? LoginSucceeded;
    public event EventHandler? CloseRequested;

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            ShowError("Please enter your username.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ShowError("Please enter your password.");
            return;
        }

        IsLoading = true;
        ClearError();

        try
        {
            var result = await _authService.LoginAsync(Username, Password);

            if (result.Success)
            {
                IsLoginSuccessful = true;
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ShowError(result.ErrorMessage ?? "Login failed. Please check your credentials.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"An error occurred: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            ShowError("Please enter a username.");
            return;
        }

        if (Username.Length < 3)
        {
            ShowError("Username must be at least 3 characters.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            ShowError("Please enter your email address.");
            return;
        }

        if (!IsValidEmail(Email))
        {
            ShowError("Please enter a valid email address.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ShowError("Please enter a password.");
            return;
        }

        if (Password.Length < 6)
        {
            ShowError("Password must be at least 6 characters.");
            return;
        }

        if (Password != ConfirmPassword)
        {
            ShowError("Passwords do not match.");
            return;
        }

        IsLoading = true;
        ClearError();

        try
        {
            var result = await _authService.RegisterAsync(Username, Email, Password);

            if (result.Success)
            {
                IsLoginSuccessful = true;
                LoginSucceeded?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ShowError(result.ErrorMessage ?? "Registration failed. Please try again.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"An error occurred: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SwitchToRegister()
    {
        IsRegisterMode = true;
        ClearError();
        ClearFields();
    }

    [RelayCommand]
    private void SwitchToLogin()
    {
        IsRegisterMode = false;
        ClearError();
        ClearFields();
    }

    [RelayCommand]
    private void Cancel()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    private void ClearFields()
    {
        Password = string.Empty;
        ConfirmPassword = string.Empty;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
