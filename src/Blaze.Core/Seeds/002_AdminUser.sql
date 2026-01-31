-- =============================================
-- Blaze Database Seed Script for SQL Server
-- Seed: 002 - Admin User
-- Target: SQL Server (localhost)
-- Database: BlazeDb
-- =============================================

USE [BlazeDb];
GO

-- =============================================
-- Seed Admin User
-- Default credentials: admin / admin123
-- Password is SHA256 hashed
-- =============================================

DECLARE @AdminId NVARCHAR(450) = 'admin-user-001';
DECLARE @Username NVARCHAR(256) = 'admin';
DECLARE @Email NVARCHAR(256) = 'admin@blaze.local';
-- SHA256 hash of 'admin123' (UTF-8 encoded)
DECLARE @PasswordHash NVARCHAR(256) = '240BE518FABD2724DDB6F04EEB1DA5967448D7E831C08C8FA822809F74C720A9';
DECLARE @DisplayName NVARCHAR(256) = 'Administrator';

-- Check if admin user already exists
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Username] = @Username)
BEGIN
    INSERT INTO [dbo].[Users] (
        [Id],
        [Username],
        [Email],
        [PasswordHash],
        [DisplayName],
        [AvatarUrl],
        [CreatedAt],
        [LastLoginAt],
        [OwnedAppIds],
        [WishlistAppIds],
        [FavoriteAppIds],
        [IsOnline],
        [IsAdmin],
        [Status],
        [StatusMessage]
    )
    VALUES (
        @AdminId,
        @Username,
        @Email,
        @PasswordHash,
        @DisplayName,
        '',
        GETUTCDATE(),
        GETUTCDATE(),
        '',
        '',
        '',
        0,
        1,  -- IsAdmin = true
        0,
        ''
    );

    PRINT 'Admin user created successfully.';
    PRINT 'Username: admin';
    PRINT 'Password: admin123';
END
ELSE
BEGIN
    -- Update existing admin user to ensure IsAdmin is set
    UPDATE [dbo].[Users]
    SET [IsAdmin] = 1
    WHERE [Username] = @Username;

    PRINT 'Admin user already exists. IsAdmin flag updated.';
END
GO

-- =============================================
-- Verify Admin User
-- =============================================
SELECT
    [Id],
    [Username],
    [Email],
    [DisplayName],
    [IsAdmin],
    [CreatedAt]
FROM [dbo].[Users]
WHERE [Username] = 'admin';
GO

PRINT 'Admin user seed completed successfully.';
GO
