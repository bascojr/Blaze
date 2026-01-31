using CommunityToolkit.Mvvm.ComponentModel;

namespace Blaze.App.ViewModels;

/// <summary>
/// Base class for all ViewModels
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public virtual Task InitializeAsync() => Task.CompletedTask;

    public virtual void Cleanup() { }
}
