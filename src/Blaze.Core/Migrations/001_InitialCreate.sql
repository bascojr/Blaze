-- =============================================
-- Blaze Database Migration Script for SQL Server
-- Version: 001 - Initial Create
-- Target: SQL Server (localhost)
-- Database: BlazeDb
-- =============================================

-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'BlazeDb')
BEGIN
    CREATE DATABASE [BlazeDb];
END
GO

USE [BlazeDb];
GO

-- =============================================
-- Table: Categories
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description] NVARCHAR(1000) NOT NULL DEFAULT '',
        [IconName] NVARCHAR(100) NOT NULL DEFAULT '',
        [DisplayOrder] INT NOT NULL DEFAULT 0,
        [ParentCategoryId] NVARCHAR(450) NOT NULL DEFAULT '',
        [IsActive] BIT NOT NULL DEFAULT 1,
        [AppCount] INT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- =============================================
-- Table: Applications
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Applications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Applications] (
        [Id] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NOT NULL,
        [Description] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [ShortDescription] NVARCHAR(500) NOT NULL DEFAULT '',
        [Developer] NVARCHAR(256) NOT NULL DEFAULT '',
        [Publisher] NVARCHAR(256) NOT NULL DEFAULT '',
        [Version] NVARCHAR(50) NOT NULL DEFAULT '1.0.0',
        [ReleaseDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastUpdated] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [Price] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [SizeInBytes] BIGINT NOT NULL DEFAULT 0,
        [DownloadUrl] NVARCHAR(2048) NOT NULL DEFAULT '',
        [ExecutablePath] NVARCHAR(500) NOT NULL DEFAULT '',
        [IconUrl] NVARCHAR(2048) NOT NULL DEFAULT '',
        [HeaderImageUrl] NVARCHAR(2048) NOT NULL DEFAULT '',
        [ScreenshotUrls] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [CategoryId] NVARCHAR(450) NOT NULL DEFAULT '',
        [Tags] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [Rating] FLOAT NOT NULL DEFAULT 0,
        [ReviewCount] INT NOT NULL DEFAULT 0,
        [DownloadCount] INT NOT NULL DEFAULT 0,
        [MinimumOsVersion] NVARCHAR(50) NOT NULL DEFAULT 'Windows 10',
        [SupportedArchitectures] NVARCHAR(100) NOT NULL DEFAULT 'x64',
        [RequiresAdmin] BIT NOT NULL DEFAULT 0,
        [Website] NVARCHAR(2048) NOT NULL DEFAULT '',
        [SupportEmail] NVARCHAR(256) NOT NULL DEFAULT '',
        [LicenseType] NVARCHAR(100) NOT NULL DEFAULT 'Freeware',
        [IsInstalled] BIT NOT NULL DEFAULT 0,
        [HasUpdate] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Applications_Categories] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([Id]) ON DELETE NO ACTION
    );
END
GO

-- Create index on CategoryId for better query performance
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Applications_CategoryId' AND object_id = OBJECT_ID('dbo.Applications'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Applications_CategoryId] ON [dbo].[Applications]([CategoryId]);
END
GO

-- =============================================
-- Table: Users
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] NVARCHAR(450) NOT NULL,
        [Username] NVARCHAR(256) NOT NULL,
        [Email] NVARCHAR(256) NOT NULL DEFAULT '',
        [PasswordHash] NVARCHAR(256) NOT NULL DEFAULT '',
        [DisplayName] NVARCHAR(256) NOT NULL DEFAULT '',
        [AvatarUrl] NVARCHAR(2048) NOT NULL DEFAULT '',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastLoginAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [OwnedAppIds] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [WishlistAppIds] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [FavoriteAppIds] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [IsOnline] BIT NOT NULL DEFAULT 1,
        [Status] INT NOT NULL DEFAULT 0,
        [StatusMessage] NVARCHAR(500) NOT NULL DEFAULT '',
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Create unique index on Username
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Username' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users]([Username]);
END
GO

-- =============================================
-- Table: InstalledApps
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InstalledApps]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[InstalledApps] (
        [Id] NVARCHAR(450) NOT NULL,
        [ApplicationId] NVARCHAR(450) NOT NULL,
        [Name] NVARCHAR(256) NOT NULL DEFAULT '',
        [InstalledVersion] NVARCHAR(50) NOT NULL DEFAULT '',
        [LatestVersion] NVARCHAR(50) NOT NULL DEFAULT '',
        [InstallPath] NVARCHAR(1000) NOT NULL DEFAULT '',
        [ExecutablePath] NVARCHAR(1000) NOT NULL DEFAULT '',
        [InstalledSizeBytes] BIGINT NOT NULL DEFAULT 0,
        [InstalledAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastLaunchedAt] DATETIME2 NOT NULL,
        [LastUpdatedAt] DATETIME2 NOT NULL,
        [TotalPlayTime] BIGINT NOT NULL DEFAULT 0,
        [LaunchCount] INT NOT NULL DEFAULT 0,
        [IconPath] NVARCHAR(1000) NOT NULL DEFAULT '',
        [IsRunning] BIT NOT NULL DEFAULT 0,
        [ProcessId] INT NULL,
        [AutoUpdate] BIT NOT NULL DEFAULT 1,
        [CreateDesktopShortcut] BIT NOT NULL DEFAULT 1,
        [CreateStartMenuShortcut] BIT NOT NULL DEFAULT 1,
        [LaunchArguments] NVARCHAR(1000) NOT NULL DEFAULT '',
        CONSTRAINT [PK_InstalledApps] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_InstalledApps_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications]([Id]) ON DELETE CASCADE
    );
END
GO

-- Create index on ApplicationId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_InstalledApps_ApplicationId' AND object_id = OBJECT_ID('dbo.InstalledApps'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_InstalledApps_ApplicationId] ON [dbo].[InstalledApps]([ApplicationId]);
END
GO

-- =============================================
-- Table: Downloads
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Downloads]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Downloads] (
        [Id] NVARCHAR(450) NOT NULL,
        [ApplicationId] NVARCHAR(450) NOT NULL,
        [ApplicationName] NVARCHAR(256) NOT NULL DEFAULT '',
        [Status] INT NOT NULL DEFAULT 0,
        [TotalBytes] BIGINT NOT NULL DEFAULT 0,
        [DownloadedBytes] BIGINT NOT NULL DEFAULT 0,
        [SpeedBytesPerSecond] FLOAT NOT NULL DEFAULT 0,
        [StartedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [CompletedAt] DATETIME2 NULL,
        [DestinationPath] NVARCHAR(1000) NOT NULL DEFAULT '',
        [TempFilePath] NVARCHAR(1000) NOT NULL DEFAULT '',
        [ErrorMessage] NVARCHAR(2000) NOT NULL DEFAULT '',
        [RetryCount] INT NOT NULL DEFAULT 0,
        [MaxRetries] INT NOT NULL DEFAULT 3,
        CONSTRAINT [PK_Downloads] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Downloads_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications]([Id]) ON DELETE CASCADE
    );
END
GO

-- Create index on ApplicationId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Downloads_ApplicationId' AND object_id = OBJECT_ID('dbo.Downloads'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Downloads_ApplicationId] ON [dbo].[Downloads]([ApplicationId]);
END
GO

-- Create index on Status for filtering downloads by status
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Downloads_Status' AND object_id = OBJECT_ID('dbo.Downloads'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Downloads_Status] ON [dbo].[Downloads]([Status]);
END
GO

-- =============================================
-- Table: Reviews
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reviews]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Reviews] (
        [Id] NVARCHAR(450) NOT NULL,
        [ApplicationId] NVARCHAR(450) NOT NULL,
        [UserId] NVARCHAR(450) NOT NULL,
        [Username] NVARCHAR(256) NOT NULL DEFAULT '',
        [UserAvatarUrl] NVARCHAR(2048) NOT NULL DEFAULT '',
        [Rating] INT NOT NULL DEFAULT 0,
        [Title] NVARCHAR(256) NOT NULL DEFAULT '',
        [Content] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [HelpfulCount] INT NOT NULL DEFAULT 0,
        [NotHelpfulCount] INT NOT NULL DEFAULT 0,
        [IsVerifiedPurchase] BIT NOT NULL DEFAULT 0,
        [UserPlayTime] BIGINT NOT NULL DEFAULT 0,
        [Recommended] BIT NOT NULL DEFAULT 1,
        [ImageUrls] NVARCHAR(MAX) NOT NULL DEFAULT '',
        CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_Reviews_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [dbo].[Applications]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
    );
END
GO

-- Create index on ApplicationId for fetching reviews by app
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reviews_ApplicationId' AND object_id = OBJECT_ID('dbo.Reviews'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Reviews_ApplicationId] ON [dbo].[Reviews]([ApplicationId]);
END
GO

-- Create index on UserId for fetching reviews by user
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Reviews_UserId' AND object_id = OBJECT_ID('dbo.Reviews'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Reviews_UserId] ON [dbo].[Reviews]([UserId]);
END
GO

-- =============================================
-- Insert Default Categories (Optional - only if needed)
-- Uncomment the following section if you want seed data
-- =============================================
/*
INSERT INTO [dbo].[Categories] ([Id], [Name], [Description], [IconName], [DisplayOrder], [IsActive])
VALUES
    ('productivity', 'Productivity', 'Office suites, note-taking, project management', 'Briefcase', 1, 1),
    ('development', 'Development', 'IDEs, code editors, programming tools', 'Code', 2, 1),
    ('graphics', 'Graphics & Design', 'Image editors, 3D modeling, design tools', 'Palette', 3, 1),
    ('multimedia', 'Multimedia', 'Video players, audio editors, media tools', 'Film', 4, 1),
    ('utilities', 'Utilities', 'System tools, file managers, utilities', 'Wrench', 5, 1),
    ('internet', 'Internet', 'Browsers, download managers, networking', 'Globe', 6, 1),
    ('security', 'Security', 'Antivirus, firewalls, privacy tools', 'Shield', 7, 1),
    ('education', 'Education', 'Learning tools, reference, educational software', 'GraduationCap', 8, 1),
    ('communication', 'Communication', 'Messaging, email clients, video conferencing', 'MessageCircle', 9, 1),
    ('business', 'Business', 'Accounting, CRM, business management', 'Building', 10, 1);
*/

-- =============================================
-- Migration Complete
-- =============================================
PRINT 'Blaze database migration completed successfully.';
GO
