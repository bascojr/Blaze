-- =============================================
-- Blaze Database Migration Script for SQL Server
-- Version: 003 - Add IsAdmin Column to Users
-- Target: SQL Server (localhost)
-- Database: BlazeDb
-- =============================================

USE [BlazeDb];
GO

-- Add IsAdmin column to Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'IsAdmin')
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [IsAdmin] BIT NOT NULL DEFAULT 0;

    PRINT 'IsAdmin column added to Users table.';
END
ELSE
BEGIN
    PRINT 'IsAdmin column already exists.';
END
GO

-- =============================================
-- Optional: Set an existing user as admin
-- Uncomment and modify the following to promote a user
-- =============================================
/*
UPDATE [dbo].[Users]
SET [IsAdmin] = 1
WHERE [Username] = 'admin';

PRINT 'User "admin" has been promoted to administrator.';
*/

-- =============================================
-- Migration Complete
-- =============================================
PRINT 'Migration 003 completed successfully.';
GO
