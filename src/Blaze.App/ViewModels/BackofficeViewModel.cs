using System.Collections.ObjectModel;
using System.IO;
using Blaze.Core.Models;
using Blaze.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace Blaze.App.ViewModels;

/// <summary>
/// ViewModel for the Backoffice admin window
/// </summary>
public partial class BackofficeViewModel : ViewModelBase
{
    private readonly IAppService _appService;

    [ObservableProperty]
    private ObservableCollection<Application> _applications = new();

    [ObservableProperty]
    private Application? _selectedApplication;

    [ObservableProperty]
    private string _appId = string.Empty;

    [ObservableProperty]
    private string _appName = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _shortDescription = string.Empty;

    [ObservableProperty]
    private string _developer = string.Empty;

    [ObservableProperty]
    private string _publisher = string.Empty;

    [ObservableProperty]
    private string _version = "1.0.0";

    [ObservableProperty]
    private decimal _price;

    [ObservableProperty]
    private string _executablePath = string.Empty;

    [ObservableProperty]
    private string _categoryId = string.Empty;

    [ObservableProperty]
    private string _tags = string.Empty;

    [ObservableProperty]
    private string _minimumOsVersion = "Windows 10";

    [ObservableProperty]
    private string _supportedArchitectures = "x64";

    [ObservableProperty]
    private bool _requiresAdmin;

    [ObservableProperty]
    private string _website = string.Empty;

    [ObservableProperty]
    private string _supportEmail = string.Empty;

    [ObservableProperty]
    private string _licenseType = "Freeware";

    [ObservableProperty]
    private string _packageFilePath = string.Empty;

    [ObservableProperty]
    private string _iconFilePath = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _screenshotPaths = new();

    [ObservableProperty]
    private ObservableCollection<Category> _categories = new();

    [ObservableProperty]
    private bool _isEditMode;

    [ObservableProperty]
    private string _formTitle = "Add New Application";

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasStatusMessage;

    public event EventHandler? CloseRequested;

    public BackofficeViewModel(IAppService appService)
    {
        _appService = appService;
    }

    public override async Task InitializeAsync()
    {
        IsLoading = true;
        try
        {
            await LoadApplicationsAsync();
            await LoadCategoriesAsync();
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadApplicationsAsync()
    {
        var apps = await _appService.GetAllAppsAsync();
        Applications.Clear();
        foreach (var app in apps)
        {
            Applications.Add(app);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var categories = await _appService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
    }

    partial void OnSelectedApplicationChanged(Application? value)
    {
        if (value != null)
        {
            LoadApplicationToForm(value);
            IsEditMode = true;
            FormTitle = $"Edit: {value.Name}";
        }
    }

    private void LoadApplicationToForm(Application app)
    {
        AppId = app.Id;
        AppName = app.Name;
        Description = app.Description;
        ShortDescription = app.ShortDescription;
        Developer = app.Developer;
        Publisher = app.Publisher;
        Version = app.Version;
        Price = app.Price;
        ExecutablePath = app.ExecutablePath;
        CategoryId = app.CategoryId;
        Tags = string.Join(", ", app.Tags);
        MinimumOsVersion = app.MinimumOsVersion;
        SupportedArchitectures = app.SupportedArchitectures;
        RequiresAdmin = app.RequiresAdmin;
        Website = app.Website;
        SupportEmail = app.SupportEmail;
        LicenseType = app.LicenseType;

        PackageFilePath = app.DownloadUrl;
        IconFilePath = app.IconUrl;
        ScreenshotPaths.Clear();
        foreach (var screenshot in app.ScreenshotUrls)
        {
            ScreenshotPaths.Add(screenshot);
        }
    }

    [RelayCommand]
    private void NewApplication()
    {
        ClearForm();
        IsEditMode = false;
        FormTitle = "Add New Application";
        SelectedApplication = null;
    }

    private void ClearForm()
    {
        AppId = string.Empty;
        AppName = string.Empty;
        Description = string.Empty;
        ShortDescription = string.Empty;
        Developer = string.Empty;
        Publisher = string.Empty;
        Version = "1.0.0";
        Price = 0;
        ExecutablePath = string.Empty;
        CategoryId = string.Empty;
        Tags = string.Empty;
        MinimumOsVersion = "Windows 10";
        SupportedArchitectures = "x64";
        RequiresAdmin = false;
        Website = string.Empty;
        SupportEmail = string.Empty;
        LicenseType = "Freeware";
        PackageFilePath = string.Empty;
        IconFilePath = string.Empty;
        ScreenshotPaths.Clear();
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void BrowsePackage()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Application Package",
            Filter = "Installer Files|*.zip;*.exe;*.msi|All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            PackageFilePath = dialog.FileName;
        }
    }

    [RelayCommand]
    private void BrowseIcon()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Application Icon",
            Filter = "Image Files|*.png;*.jpg;*.jpeg;*.ico|All Files|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            IconFilePath = dialog.FileName;
        }
    }

    [RelayCommand]
    private void BrowseScreenshots()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select Screenshots",
            Filter = "Image Files|*.png;*.jpg;*.jpeg|All Files|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            foreach (var file in dialog.FileNames)
            {
                if (!ScreenshotPaths.Contains(file))
                {
                    ScreenshotPaths.Add(file);
                }
            }
        }
    }

    [RelayCommand]
    private void RemoveScreenshot(string path)
    {
        ScreenshotPaths.Remove(path);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (!ValidateForm()) return;

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var app = new Application
            {
                Id = IsEditMode ? AppId : string.Empty,
                Name = AppName,
                Description = Description,
                ShortDescription = ShortDescription,
                Developer = Developer,
                Publisher = Publisher,
                Version = Version,
                Price = Price,
                ExecutablePath = ExecutablePath,
                CategoryId = CategoryId,
                Tags = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(t => t.Trim()).ToList(),
                MinimumOsVersion = MinimumOsVersion,
                SupportedArchitectures = SupportedArchitectures,
                RequiresAdmin = RequiresAdmin,
                Website = Website,
                SupportEmail = SupportEmail,
                LicenseType = LicenseType
            };

            if (IsEditMode)
            {
                var success = await _appService.UpdateAppMetadataAsync(app);
                if (success)
                {
                    await SaveFilesAsync(app.Id);
                    ShowStatus("Application updated successfully!");
                }
                else
                {
                    ErrorMessage = "Failed to update application.";
                }
            }
            else
            {
                var created = await _appService.CreateAppAsync(app);
                await SaveFilesAsync(created.Id);
                ShowStatus("Application created successfully!");
                AppId = created.Id;
                IsEditMode = true;
                FormTitle = $"Edit: {created.Name}";
            }

            await LoadApplicationsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving application: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveFilesAsync(string appId)
    {
        if (!string.IsNullOrEmpty(PackageFilePath) && File.Exists(PackageFilePath))
        {
            await _appService.SavePackageFileAsync(appId, PackageFilePath);
        }

        if (!string.IsNullOrEmpty(IconFilePath) && File.Exists(IconFilePath))
        {
            await _appService.SaveIconAsync(appId, IconFilePath);
        }

        var newScreenshots = ScreenshotPaths.Where(File.Exists).ToList();
        if (newScreenshots.Any())
        {
            await _appService.SaveScreenshotsAsync(appId, newScreenshots);
        }
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(AppName))
        {
            ErrorMessage = "Application name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Developer))
        {
            ErrorMessage = "Developer name is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            ErrorMessage = "Description is required.";
            return false;
        }

        if (!IsEditMode && string.IsNullOrWhiteSpace(PackageFilePath))
        {
            ErrorMessage = "Package file is required for new applications.";
            return false;
        }

        return true;
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (!IsEditMode || string.IsNullOrEmpty(AppId)) return;

        IsLoading = true;
        try
        {
            var success = await _appService.DeleteAppAsync(AppId);
            if (success)
            {
                ShowStatus("Application deleted successfully!");
                ClearForm();
                IsEditMode = false;
                FormTitle = "Add New Application";
                await LoadApplicationsAsync();
            }
            else
            {
                ErrorMessage = "Failed to delete application.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error deleting application: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ShowStatus(string message)
    {
        StatusMessage = message;
        HasStatusMessage = true;

        Task.Delay(3000).ContinueWith(_ =>
        {
            StatusMessage = string.Empty;
            HasStatusMessage = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public override void Cleanup()
    {
        Applications.Clear();
        Categories.Clear();
    }
}
