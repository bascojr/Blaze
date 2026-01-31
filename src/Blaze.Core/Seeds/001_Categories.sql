-- =============================================
-- Blaze Database Seed Script for SQL Server
-- Seed: 001 - Categories
-- Target: SQL Server (localhost)
-- Database: BlazeDb
-- =============================================

USE [BlazeDb];
GO

-- =============================================
-- Seed Categories
-- Uses MERGE to insert or update existing records
-- =============================================

MERGE INTO [dbo].[Categories] AS target
USING (VALUES
    ('productivity', 'Productivity', 'Office suites, note-taking, project management', 'Briefcase', 1, '', 1, 0),
    ('development', 'Development', 'IDEs, code editors, programming tools', 'Code', 2, '', 1, 0),
    ('graphics', 'Graphics & Design', 'Image editors, 3D modeling, design tools', 'Palette', 3, '', 1, 0),
    ('multimedia', 'Multimedia', 'Video players, audio editors, media tools', 'Film', 4, '', 1, 0),
    ('utilities', 'Utilities', 'System tools, file managers, utilities', 'Wrench', 5, '', 1, 0),
    ('internet', 'Internet', 'Browsers, download managers, networking', 'Globe', 6, '', 1, 0),
    ('security', 'Security', 'Antivirus, firewalls, privacy tools', 'Shield', 7, '', 1, 0),
    ('education', 'Education', 'Learning tools, reference, educational software', 'GraduationCap', 8, '', 1, 0),
    ('communication', 'Communication', 'Messaging, email clients, video conferencing', 'MessageCircle', 9, '', 1, 0),
    ('business', 'Business', 'Accounting, CRM, business management', 'Building', 10, '', 1, 0)
) AS source ([Id], [Name], [Description], [IconName], [DisplayOrder], [ParentCategoryId], [IsActive], [AppCount])
ON target.[Id] = source.[Id]
WHEN MATCHED THEN
    UPDATE SET
        [Name] = source.[Name],
        [Description] = source.[Description],
        [IconName] = source.[IconName],
        [DisplayOrder] = source.[DisplayOrder],
        [IsActive] = source.[IsActive]
WHEN NOT MATCHED THEN
    INSERT ([Id], [Name], [Description], [IconName], [DisplayOrder], [ParentCategoryId], [IsActive], [AppCount])
    VALUES (source.[Id], source.[Name], source.[Description], source.[IconName], source.[DisplayOrder], source.[ParentCategoryId], source.[IsActive], source.[AppCount]);
GO

-- =============================================
-- Verify Seed Data
-- =============================================
SELECT
    [Id],
    [Name],
    [Description],
    [IconName],
    [DisplayOrder],
    [IsActive]
FROM [dbo].[Categories]
ORDER BY [DisplayOrder];
GO

PRINT 'Categories seed completed successfully. 10 categories inserted/updated.';
GO
