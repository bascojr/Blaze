using System.IO;
using System.Windows;
using Blaze.App.ViewModels;
using Blaze.App.Views;
using Blaze.Core.Data;
using Blaze.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blaze.App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Configure services
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Initialize database
        var dbContext = Services.GetRequiredService<BlazeDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // Try to restore session
        var authService = Services.GetRequiredService<IAuthService>();
        var sessionValid = await authService.ValidateSessionAsync();

        // If no valid session, show login window
        if (!sessionValid)
        {
            var loginWindow = new LoginWindow();
            var result = loginWindow.ShowDialog();

            // If login was not successful, exit the application
            if (result != true || !loginWindow.IsLoginSuccessful)
            {
                Shutdown();
                return;
            }
        }

        // Show main window after successful authentication
        var mainWindow = new MainWindow();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Logging
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddDebug();
        });

        // Database
        var connectionString = configuration.GetConnectionString("BlazeDb");
        services.AddDbContext<BlazeDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddScoped<IAppService, AppService>();
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IAuthService, AuthService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<StoreViewModel>();
        services.AddTransient<LibraryViewModel>();
        services.AddTransient<DownloadsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AppDetailViewModel>();
        services.AddTransient<BackofficeViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        // Cleanup
        if (Services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
