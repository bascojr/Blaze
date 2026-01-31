# Blaze - Windows App Store

A Steam-like application store for Windows applications built with .NET 8 and WPF.

## Features

- **Store**: Browse and discover Windows applications with categories, search, and sorting
- **Library**: Manage your installed applications with launch, update, and uninstall capabilities
- **Downloads**: Queue-based download manager with pause, resume, and speed limiting
- **Settings**: Comprehensive settings for downloads, notifications, interface, and library management
- **Usage Tracking**: Track application usage time and launch history
- **Favorites & Wishlist**: Organize your apps and save apps for later

## Project Structure

```
Blaze/
├── Blaze.sln                    # Solution file
├── src/
│   ├── Blaze.Core/              # Core library
│   │   ├── Models/              # Data models (Application, User, Download, etc.)
│   │   ├── Services/            # Business logic services
│   │   ├── Data/                # Entity Framework DbContext
│   │   ├── Events/              # Event arguments
│   │   └── Helpers/             # Utility classes
│   │
│   └── Blaze.App/               # WPF Application
│       ├── Views/               # XAML pages and windows
│       ├── ViewModels/          # MVVM ViewModels
│       ├── Controls/            # Custom controls
│       ├── Converters/          # Value converters
│       ├── Resources/           # Styles and themes
│       └── Assets/              # Images and icons
```

## Technologies

- **.NET 8** - Target framework
- **WPF** - Windows Presentation Foundation for UI
- **Entity Framework Core** - SQLite database for local storage
- **CommunityToolkit.Mvvm** - MVVM framework
- **Microsoft.Extensions.DependencyInjection** - Dependency injection

## Getting Started

### Prerequisites

- Windows 10/11
- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# extension

### Building

```bash
# Clone the repository
git clone <repository-url>
cd Blaze

# Restore packages and build
dotnet restore
dotnet build

# Run the application
dotnet run --project src/Blaze.App
```

## Architecture

### Core Library (Blaze.Core)

Contains all business logic and data models:

- **Models**: Application, Category, User, Download, InstalledApp, Settings, Review
- **Services**: IAppService, IDownloadService, IAuthService, ISettingsService
- **Data**: SQLite database with Entity Framework Core

### WPF Application (Blaze.App)

MVVM architecture with:

- **Views**: StorePage, LibraryPage, DownloadsPage, SettingsPage, AppDetailPage
- **ViewModels**: Handle UI logic and data binding
- **Resources**: Steam-inspired dark theme with blue accent colors

## Key Features Implementation

### Download Manager

- Queue-based download system with concurrent download limits
- Pause, resume, and cancel functionality
- Speed limiting and progress tracking
- Automatic installation after download

### App Library

- Grid and list view options
- Sort by name, recent, install date, size, or play time
- Filter by all, installed, has updates, or favorites
- Usage time tracking

### Store

- Category-based browsing
- Search functionality
- Sort by popular, new releases, name, or rating
- App details with ratings, reviews, and similar apps

## Theme

The application uses a Steam-inspired dark theme with:

- Dark background colors (#171a21, #1b2838)
- Blue accent colors (#1a9fff, #66c0f4)
- High contrast text for readability
- Rounded corners and modern UI elements

## License

MIT License
