-- =============================================
-- Blaze Database Migration Script for SQL Server
-- Version: 002 - Add PasswordHash Column
-- Target: SQL Server (localhost)
-- Database: BlazeDb
-- =============================================

USE [BlazeDb];
GO

-- Add PasswordHash column to Users table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'PasswordHash')
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [PasswordHash] NVARCHAR(256) NOT NULL DEFAULT '';

    PRINT 'PasswordHash column added to Users table.';
END
ELSE
BEGIN
    PRINT 'PasswordHash column already exists.';
END
GO

-- =============================================
-- Migration Complete
-- =============================================
PRINT 'Migration 002 completed successfully.';
GO
