-- =============================================
-- Blaze Database Seed Script for SQL Server
-- Seed: 003 - Sample Applications
-- Target: SQL Server (localhost)
-- Database: BlazeDb
-- =============================================

USE [BlazeDb];
GO

-- =============================================
-- Seed Sample Applications
-- =============================================

-- Ensure categories exist first
IF NOT EXISTS (SELECT 1 FROM [dbo].[Categories] WHERE [Id] = 'productivity')
BEGIN
    PRINT 'ERROR: Categories must be seeded first. Run 001_Categories.sql';
    RETURN;
END

MERGE INTO [dbo].[Applications] AS target
USING (VALUES
    -- Productivity Apps
    ('app-notepad-plus', 'Notepad++', 'Notepad++ is a free source code editor and Notepad replacement that supports several languages.', 'Fast and lightweight text editor', 'Don Ho', 'Don Ho', '8.6.0', '2024-01-15', '2024-01-15', 0, 5242880, '', 'notepad++.exe', '', '', '', 'productivity', 'text editor,code,developer,notepad', 4.8, 1250, 50000, 'Windows 10', 'x64,x86', 0, 'https://notepad-plus-plus.org', 'support@notepad-plus-plus.org', 'GPL', 0, 0),
    ('app-libreoffice', 'LibreOffice', 'LibreOffice is a powerful and free office suite, used by millions of people around the world.', 'Free and open source office suite', 'The Document Foundation', 'The Document Foundation', '7.6.4', '2024-01-10', '2024-01-10', 0, 367001600, '', 'soffice.exe', '', '', '', 'productivity', 'office,documents,spreadsheet,presentation', 4.5, 890, 35000, 'Windows 10', 'x64', 0, 'https://www.libreoffice.org', 'support@libreoffice.org', 'MPL-2.0', 0, 0),

    -- Development Apps
    ('app-vscode', 'Visual Studio Code', 'Visual Studio Code is a lightweight but powerful source code editor which runs on your desktop.', 'Code editing redefined', 'Microsoft', 'Microsoft', '1.85.0', '2024-01-12', '2024-01-12', 0, 104857600, '', 'Code.exe', '', '', '', 'development', 'ide,code editor,developer,programming', 4.9, 5420, 150000, 'Windows 10', 'x64,arm64', 0, 'https://code.visualstudio.com', 'support@microsoft.com', 'MIT', 0, 0),
    ('app-git', 'Git for Windows', 'Git is a free and open source distributed version control system designed to handle everything from small to very large projects.', 'Distributed version control', 'Git Community', 'Git Community', '2.43.0', '2024-01-08', '2024-01-08', 0, 52428800, '', 'git-bash.exe', '', '', '', 'development', 'git,version control,developer,scm', 4.7, 2100, 80000, 'Windows 10', 'x64', 0, 'https://git-scm.com', 'git@vger.kernel.org', 'GPL-2.0', 0, 0),
    ('app-nodejs', 'Node.js', 'Node.js is a JavaScript runtime built on Chrome''s V8 JavaScript engine for building scalable network applications.', 'JavaScript runtime environment', 'OpenJS Foundation', 'OpenJS Foundation', '20.11.0', '2024-01-14', '2024-01-14', 0, 31457280, '', 'node.exe', '', '', '', 'development', 'nodejs,javascript,runtime,developer', 4.6, 1800, 95000, 'Windows 10', 'x64', 0, 'https://nodejs.org', 'support@nodejs.org', 'MIT', 0, 0),

    -- Graphics Apps
    ('app-gimp', 'GIMP', 'GIMP is a cross-platform image editor available for GNU/Linux, macOS, Windows and more operating systems.', 'Free image editor', 'GIMP Team', 'GIMP Team', '2.10.36', '2024-01-05', '2024-01-05', 0, 262144000, '', 'gimp-2.10.exe', '', '', '', 'graphics', 'image editor,photo,design,graphics', 4.4, 980, 42000, 'Windows 10', 'x64', 0, 'https://www.gimp.org', 'support@gimp.org', 'GPL-3.0', 0, 0),
    ('app-inkscape', 'Inkscape', 'Inkscape is a free and open-source vector graphics editor used to create vector images, primarily in SVG format.', 'Vector graphics editor', 'Inkscape Community', 'Inkscape Community', '1.3.2', '2024-01-03', '2024-01-03', 0, 157286400, '', 'inkscape.exe', '', '', '', 'graphics', 'vector,svg,graphics,design,illustration', 4.5, 720, 28000, 'Windows 10', 'x64', 0, 'https://inkscape.org', 'support@inkscape.org', 'GPL-3.0', 0, 0),

    -- Multimedia Apps
    ('app-vlc', 'VLC Media Player', 'VLC is a free and open source cross-platform multimedia player and framework that plays most multimedia files.', 'Universal media player', 'VideoLAN', 'VideoLAN', '3.0.20', '2024-01-11', '2024-01-11', 0, 41943040, '', 'vlc.exe', '', '', '', 'multimedia', 'video player,media,audio,streaming', 4.8, 3200, 200000, 'Windows 10', 'x64,x86', 0, 'https://www.videolan.org', 'support@videolan.org', 'GPL-2.0', 0, 0),
    ('app-audacity', 'Audacity', 'Audacity is a free, easy-to-use, multi-track audio editor and recorder for Windows, macOS, GNU/Linux and other OS.', 'Free audio editor', 'Audacity Team', 'Audacity Team', '3.4.2', '2024-01-09', '2024-01-09', 0, 36700160, '', 'audacity.exe', '', '', '', 'multimedia', 'audio editor,recording,podcast,music', 4.6, 1560, 75000, 'Windows 10', 'x64', 0, 'https://www.audacityteam.org', 'support@audacityteam.org', 'GPL-3.0', 0, 0),

    -- Utilities Apps
    ('app-7zip', '7-Zip', '7-Zip is a file archiver with a high compression ratio supporting 7z, ZIP, RAR, and other archive formats.', 'High compression file archiver', 'Igor Pavlov', 'Igor Pavlov', '23.01', '2024-01-06', '2024-01-06', 0, 1572864, '', '7zFM.exe', '', '', '', 'utilities', 'compression,archive,zip,rar,7z', 4.9, 4100, 180000, 'Windows 10', 'x64,x86', 0, 'https://www.7-zip.org', 'support@7-zip.org', 'LGPL-2.1', 0, 0),
    ('app-everything', 'Everything', 'Everything is a desktop search utility for Windows that can rapidly find files and folders by name.', 'Instant file search', 'voidtools', 'voidtools', '1.4.1', '2024-01-04', '2024-01-04', 0, 1835008, '', 'Everything.exe', '', '', '', 'utilities', 'search,files,finder,indexer', 4.9, 2800, 120000, 'Windows 10', 'x64,x86', 0, 'https://www.voidtools.com', 'support@voidtools.com', 'MIT', 0, 0),

    -- Internet Apps
    ('app-firefox', 'Mozilla Firefox', 'Firefox is a free and open-source web browser developed by the Mozilla Foundation.', 'Fast and private browser', 'Mozilla', 'Mozilla Foundation', '122.0', '2024-01-16', '2024-01-16', 0, 57671680, '', 'firefox.exe', '', '', '', 'internet', 'browser,web,internet,privacy', 4.6, 2900, 140000, 'Windows 10', 'x64', 0, 'https://www.mozilla.org/firefox', 'support@mozilla.org', 'MPL-2.0', 0, 0),
    ('app-thunderbird', 'Mozilla Thunderbird', 'Thunderbird is a free email application that''s easy to set up and customize with great features.', 'Free email client', 'Mozilla', 'Mozilla Foundation', '115.7.0', '2024-01-13', '2024-01-13', 0, 52428800, '', 'thunderbird.exe', '', '', '', 'internet', 'email,mail client,calendar,contacts', 4.5, 1100, 55000, 'Windows 10', 'x64', 0, 'https://www.thunderbird.net', 'support@mozilla.org', 'MPL-2.0', 0, 0),

    -- Security Apps
    ('app-keepass', 'KeePass', 'KeePass is a free open source password manager, which helps you to manage your passwords in a secure way.', 'Secure password manager', 'Dominik Reichl', 'Dominik Reichl', '2.56', '2024-01-07', '2024-01-07', 0, 3145728, '', 'KeePass.exe', '', '', '', 'security', 'password manager,security,encryption,vault', 4.7, 1650, 65000, 'Windows 10', 'x64,x86', 0, 'https://keepass.info', 'support@keepass.info', 'GPL-2.0', 0, 0),

    -- Communication Apps
    ('app-signal', 'Signal Desktop', 'Signal is a cross-platform encrypted messaging service with a focus on privacy and security.', 'Private messenger', 'Signal Foundation', 'Signal Foundation', '6.48.0', '2024-01-17', '2024-01-17', 0, 157286400, '', 'Signal.exe', '', '', '', 'communication', 'messaging,encrypted,privacy,chat', 4.8, 1900, 70000, 'Windows 10', 'x64', 0, 'https://signal.org', 'support@signal.org', 'AGPL-3.0', 0, 0)
) AS source (
    [Id], [Name], [Description], [ShortDescription], [Developer], [Publisher], [Version],
    [ReleaseDate], [LastUpdated], [Price], [SizeInBytes], [DownloadUrl], [ExecutablePath],
    [IconUrl], [HeaderImageUrl], [ScreenshotUrls], [CategoryId], [Tags], [Rating],
    [ReviewCount], [DownloadCount], [MinimumOsVersion], [SupportedArchitectures],
    [RequiresAdmin], [Website], [SupportEmail], [LicenseType], [IsInstalled], [HasUpdate]
)
ON target.[Id] = source.[Id]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = source.[Name],
        [Description] = source.[Description],
        [ShortDescription] = source.[ShortDescription],
        [Developer] = source.[Developer],
        [Publisher] = source.[Publisher],
        [Version] = source.[Version],
        [LastUpdated] = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (
        [Id], [Name], [Description], [ShortDescription], [Developer], [Publisher], [Version],
        [ReleaseDate], [LastUpdated], [Price], [SizeInBytes], [DownloadUrl], [ExecutablePath],
        [IconUrl], [HeaderImageUrl], [ScreenshotUrls], [CategoryId], [Tags], [Rating],
        [ReviewCount], [DownloadCount], [MinimumOsVersion], [SupportedArchitectures],
        [RequiresAdmin], [Website], [SupportEmail], [LicenseType], [IsInstalled], [HasUpdate]
    )
    VALUES (
        source.[Id], source.[Name], source.[Description], source.[ShortDescription],
        source.[Developer], source.[Publisher], source.[Version], source.[ReleaseDate],
        source.[LastUpdated], source.[Price], source.[SizeInBytes], source.[DownloadUrl],
        source.[ExecutablePath], source.[IconUrl], source.[HeaderImageUrl], source.[ScreenshotUrls],
        source.[CategoryId], source.[Tags], source.[Rating], source.[ReviewCount],
        source.[DownloadCount], source.[MinimumOsVersion], source.[SupportedArchitectures],
        source.[RequiresAdmin], source.[Website], source.[SupportEmail], source.[LicenseType],
        source.[IsInstalled], source.[HasUpdate]
    );
GO

-- =============================================
-- Update Category App Counts
-- =============================================
UPDATE c
SET c.[AppCount] = (SELECT COUNT(*) FROM [dbo].[Applications] a WHERE a.[CategoryId] = c.[Id])
FROM [dbo].[Categories] c;
GO

-- =============================================
-- Verify Sample Applications
-- =============================================
SELECT
    a.[Name],
    c.[Name] AS Category,
    a.[Developer],
    a.[Rating],
    a.[DownloadCount]
FROM [dbo].[Applications] a
LEFT JOIN [dbo].[Categories] c ON a.[CategoryId] = c.[Id]
ORDER BY c.[DisplayOrder], a.[Name];
GO

PRINT 'Sample applications seed completed successfully. 15 applications inserted/updated.';
GO
