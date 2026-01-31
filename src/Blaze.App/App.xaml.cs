using System.Windows;
using Blaze.App.ViewModels;
using Blaze.Core.Data;
using Blaze.Core.Services;
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
        await dbContext.EnsureCreatedAsync();

        // Seed demo data
        await SeedDemoDataAsync(dbContext);

        // Try to restore session
        var authService = Services.GetRequiredService<IAuthService>();
        await authService.ValidateSessionAsync();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddDebug();
        });

        // Database
        services.AddDbContext<BlazeDbContext>();

        // Services
        services.AddSingleton<ISettingsService, SettingsService>();
        services.AddScoped<IAppService, AppService>();
        services.AddScoped<IDownloadService, DownloadService>();
        services.AddScoped<IAuthService, AuthService>();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<StoreViewModel>();
        services.AddTransient<LibraryViewModel>();
        services.AddTransient<DownloadsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AppDetailViewModel>();
    }

    private static async Task SeedDemoDataAsync(BlazeDbContext context)
    {
        // Check if data already exists
        if (context.Applications.Any()) return;

        // Add demo applications
        var demoApps = new[]
        {
            new Core.Models.Application
            {
                Id = "vscode",
                Name = "Visual Studio Code",
                ShortDescription = "Lightweight but powerful source code editor",
                Description = "Visual Studio Code is a lightweight but powerful source code editor which runs on your desktop. It comes with built-in support for JavaScript, TypeScript and Node.js and has a rich ecosystem of extensions for other languages.",
                Developer = "Microsoft",
                Publisher = "Microsoft Corporation",
                Version = "1.85.0",
                ReleaseDate = DateTime.Now.AddMonths(-24),
                LastUpdated = DateTime.Now.AddDays(-5),
                Price = 0,
                SizeInBytes = 95_000_000,
                CategoryId = "development",
                Tags = new List<string> { "IDE", "Code Editor", "Development", "Programming" },
                Rating = 4.8,
                ReviewCount = 15420,
                DownloadCount = 5_200_000,
                MinimumOsVersion = "Windows 10",
                Website = "https://code.visualstudio.com",
                LicenseType = "MIT License",
                ExecutablePath = "Code.exe"
            },
            new Core.Models.Application
            {
                Id = "firefox",
                Name = "Mozilla Firefox",
                ShortDescription = "Fast, private and secure web browser",
                Description = "Firefox is more than a browser. It's a commitment to a healthier internet. Firefox blocks over 2000 trackers by default, giving you phenomenal privacy protection.",
                Developer = "Mozilla",
                Publisher = "Mozilla Foundation",
                Version = "121.0",
                ReleaseDate = DateTime.Now.AddMonths(-120),
                LastUpdated = DateTime.Now.AddDays(-2),
                Price = 0,
                SizeInBytes = 55_000_000,
                CategoryId = "internet",
                Tags = new List<string> { "Browser", "Web", "Internet", "Privacy" },
                Rating = 4.5,
                ReviewCount = 25600,
                DownloadCount = 12_000_000,
                MinimumOsVersion = "Windows 10",
                Website = "https://www.mozilla.org/firefox",
                LicenseType = "MPL 2.0",
                ExecutablePath = "firefox.exe"
            },
            new Core.Models.Application
            {
                Id = "vlc",
                Name = "VLC Media Player",
                ShortDescription = "Free and open source multimedia player",
                Description = "VLC is a free and open source cross-platform multimedia player and framework that plays most multimedia files as well as DVDs, Audio CDs, VCDs, and various streaming protocols.",
                Developer = "VideoLAN",
                Publisher = "VideoLAN Organization",
                Version = "3.0.20",
                ReleaseDate = DateTime.Now.AddMonths(-60),
                LastUpdated = DateTime.Now.AddDays(-30),
                Price = 0,
                SizeInBytes = 45_000_000,
                CategoryId = "multimedia",
                Tags = new List<string> { "Media Player", "Video", "Audio", "Streaming" },
                Rating = 4.7,
                ReviewCount = 32100,
                DownloadCount = 25_000_000,
                MinimumOsVersion = "Windows 7",
                Website = "https://www.videolan.org/vlc",
                LicenseType = "GPL",
                ExecutablePath = "vlc.exe"
            },
            new Core.Models.Application
            {
                Id = "gimp",
                Name = "GIMP",
                ShortDescription = "GNU Image Manipulation Program",
                Description = "GIMP is a cross-platform image editor available for GNU/Linux, macOS, Windows and more operating systems. It is free software, you can change its source code and distribute your changes.",
                Developer = "The GIMP Team",
                Publisher = "GIMP Development Team",
                Version = "2.10.36",
                ReleaseDate = DateTime.Now.AddMonths(-180),
                LastUpdated = DateTime.Now.AddDays(-45),
                Price = 0,
                SizeInBytes = 250_000_000,
                CategoryId = "graphics",
                Tags = new List<string> { "Image Editor", "Graphics", "Photo", "Design" },
                Rating = 4.4,
                ReviewCount = 18900,
                DownloadCount = 8_500_000,
                MinimumOsVersion = "Windows 10",
                Website = "https://www.gimp.org",
                LicenseType = "GPL",
                ExecutablePath = "gimp-2.10.exe"
            },
            new Core.Models.Application
            {
                Id = "7zip",
                Name = "7-Zip",
                ShortDescription = "File archiver with high compression ratio",
                Description = "7-Zip is a file archiver with a high compression ratio. The main features of 7-Zip include high compression ratio in 7z format, support for packing/unpacking various formats, and strong AES-256 encryption.",
                Developer = "Igor Pavlov",
                Publisher = "Igor Pavlov",
                Version = "23.01",
                ReleaseDate = DateTime.Now.AddMonths(-200),
                LastUpdated = DateTime.Now.AddDays(-60),
                Price = 0,
                SizeInBytes = 1_500_000,
                CategoryId = "utilities",
                Tags = new List<string> { "Archive", "Compression", "ZIP", "Utility" },
                Rating = 4.9,
                ReviewCount = 45200,
                DownloadCount = 50_000_000,
                MinimumOsVersion = "Windows 7",
                Website = "https://www.7-zip.org",
                LicenseType = "LGPL",
                ExecutablePath = "7zFM.exe"
            },
            new Core.Models.Application
            {
                Id = "notepadpp",
                Name = "Notepad++",
                ShortDescription = "Free source code editor and Notepad replacement",
                Description = "Notepad++ is a free source code editor and Notepad replacement that supports several languages. Running in the MS Windows environment, its use is governed by GPL License.",
                Developer = "Don Ho",
                Publisher = "Don Ho",
                Version = "8.6.2",
                ReleaseDate = DateTime.Now.AddMonths(-180),
                LastUpdated = DateTime.Now.AddDays(-15),
                Price = 0,
                SizeInBytes = 5_000_000,
                CategoryId = "development",
                Tags = new List<string> { "Text Editor", "Code Editor", "Development" },
                Rating = 4.8,
                ReviewCount = 28700,
                DownloadCount = 35_000_000,
                MinimumOsVersion = "Windows 7",
                Website = "https://notepad-plus-plus.org",
                LicenseType = "GPL",
                ExecutablePath = "notepad++.exe"
            },
            new Core.Models.Application
            {
                Id = "obs",
                Name = "OBS Studio",
                ShortDescription = "Free and open source software for video recording and live streaming",
                Description = "OBS Studio is a free and open source software for video recording and live streaming. Stream to Twitch, YouTube and many other providers or record your own videos with high quality H264 / AAC encoding.",
                Developer = "OBS Project",
                Publisher = "OBS Project",
                Version = "30.0.2",
                ReleaseDate = DateTime.Now.AddMonths(-96),
                LastUpdated = DateTime.Now.AddDays(-10),
                Price = 0,
                SizeInBytes = 180_000_000,
                CategoryId = "multimedia",
                Tags = new List<string> { "Streaming", "Recording", "Video", "Broadcasting" },
                Rating = 4.7,
                ReviewCount = 21300,
                DownloadCount = 15_000_000,
                MinimumOsVersion = "Windows 10",
                Website = "https://obsproject.com",
                LicenseType = "GPL",
                ExecutablePath = "obs64.exe"
            },
            new Core.Models.Application
            {
                Id = "libreoffice",
                Name = "LibreOffice",
                ShortDescription = "Free and powerful office suite",
                Description = "LibreOffice is a powerful and free office suite, used by millions of people around the world. Its clean interface and feature-rich tools help you unleash your creativity and enhance your productivity.",
                Developer = "The Document Foundation",
                Publisher = "The Document Foundation",
                Version = "7.6.4",
                ReleaseDate = DateTime.Now.AddMonths(-150),
                LastUpdated = DateTime.Now.AddDays(-20),
                Price = 0,
                SizeInBytes = 350_000_000,
                CategoryId = "productivity",
                Tags = new List<string> { "Office Suite", "Documents", "Spreadsheets", "Presentations" },
                Rating = 4.5,
                ReviewCount = 19800,
                DownloadCount = 18_000_000,
                MinimumOsVersion = "Windows 7",
                Website = "https://www.libreoffice.org",
                LicenseType = "MPL 2.0",
                ExecutablePath = "soffice.exe"
            },
            new Core.Models.Application
            {
                Id = "keepass",
                Name = "KeePass",
                ShortDescription = "Free, open source, light-weight password manager",
                Description = "KeePass is a free open source password manager, which helps you to manage your passwords in a secure way. You can store all your passwords in one database, which is locked with a master key.",
                Developer = "Dominik Reichl",
                Publisher = "Dominik Reichl",
                Version = "2.55",
                ReleaseDate = DateTime.Now.AddMonths(-200),
                LastUpdated = DateTime.Now.AddDays(-40),
                Price = 0,
                SizeInBytes = 4_000_000,
                CategoryId = "security",
                Tags = new List<string> { "Password Manager", "Security", "Encryption" },
                Rating = 4.6,
                ReviewCount = 12500,
                DownloadCount = 10_000_000,
                MinimumOsVersion = "Windows 7",
                Website = "https://keepass.info",
                LicenseType = "GPL",
                ExecutablePath = "KeePass.exe"
            },
            new Core.Models.Application
            {
                Id = "discord",
                Name = "Discord",
                ShortDescription = "Voice, video, and text communication platform",
                Description = "Discord is the easiest way to talk over voice, video, and text. Talk, chat, hang out, and stay close with your friends and communities.",
                Developer = "Discord Inc.",
                Publisher = "Discord Inc.",
                Version = "1.0.9024",
                ReleaseDate = DateTime.Now.AddMonths(-96),
                LastUpdated = DateTime.Now.AddDays(-3),
                Price = 0,
                SizeInBytes = 120_000_000,
                CategoryId = "communication",
                Tags = new List<string> { "Chat", "Voice", "Communication", "Gaming" },
                Rating = 4.6,
                ReviewCount = 35600,
                DownloadCount = 40_000_000,
                MinimumOsVersion = "Windows 10",
                Website = "https://discord.com",
                LicenseType = "Proprietary",
                ExecutablePath = "Discord.exe"
            },
            new Core.Models.Application
            {
                Id = "blender",
                Name = "Blender",
                ShortDescription = "Free and open source 3D creation suite",
                Description = "Blender is the free and open source 3D creation suite. It supports the entirety of the 3D pipeline—modeling, rigging, animation, simulation, rendering, compositing and motion tracking.",
                Developer = "Blender Foundation",
                Publisher = "Blender Foundation",
                Version = "4.0.2",
                ReleaseDate = DateTime.Now.AddMonths(-240),
                LastUpdated = DateTime.Now.AddDays(-7),
                Price = 0,
                SizeInBytes = 450_000_000,
                CategoryId = "graphics",
                Tags = new List<string> { "3D", "Modeling", "Animation", "Rendering" },
                Rating = 4.8,
                ReviewCount = 24100,
                DownloadCount = 12_000_000,
                MinimumOsVersion = "Windows 10",
                Website = "https://www.blender.org",
                LicenseType = "GPL",
                ExecutablePath = "blender.exe"
            },
            new Core.Models.Application
            {
                Id = "audacity",
                Name = "Audacity",
                ShortDescription = "Free, open source, cross-platform audio software",
                Description = "Audacity is an easy-to-use, multi-track audio editor and recorder for Windows, macOS, GNU/Linux and other operating systems.",
                Developer = "Audacity Team",
                Publisher = "Audacity Team",
                Version = "3.4.2",
                ReleaseDate = DateTime.Now.AddMonths(-260),
                LastUpdated = DateTime.Now.AddDays(-25),
                Price = 0,
                SizeInBytes = 35_000_000,
                CategoryId = "multimedia",
                Tags = new List<string> { "Audio", "Editor", "Recording", "Music" },
                Rating = 4.5,
                ReviewCount = 28900,
                DownloadCount = 22_000_000,
                MinimumOsVersion = "Windows 10",
                Website = "https://www.audacityteam.org",
                LicenseType = "GPL",
                ExecutablePath = "audacity.exe"
            }
        };

        context.Applications.AddRange(demoApps);
        await context.SaveChangesAsync();
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
