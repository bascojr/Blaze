-- =============================================
-- Blaze Database - Run All Seeds
-- Target: SQL Server (localhost)
-- Database: BlazeDb
-- =============================================
--
-- This script runs all seed files in the correct order.
-- Make sure migrations have been applied first!
--
-- Usage:
--   sqlcmd -S localhost -d BlazeDb -i "RunAllSeeds.sql"
--
-- Or run each file individually in order:
--   1. 001_Categories.sql
--   2. 002_AdminUser.sql
--   3. 003_SampleApplications.sql
-- =============================================

USE [BlazeDb];
GO

PRINT '================================================';
PRINT 'Starting Blaze Database Seeding...';
PRINT '================================================';
PRINT '';

-- =============================================
-- 1. Seed Categories
-- =============================================
PRINT 'Seeding Categories...';
:r 001_Categories.sql
PRINT '';

-- =============================================
-- 2. Seed Admin User
-- =============================================
PRINT 'Seeding Admin User...';
:r 002_AdminUser.sql
PRINT '';

-- =============================================
-- 3. Seed Sample Applications
-- =============================================
PRINT 'Seeding Sample Applications...';
:r 003_SampleApplications.sql
PRINT '';

-- =============================================
-- Summary
-- =============================================
PRINT '================================================';
PRINT 'Seeding Complete!';
PRINT '================================================';
PRINT '';

SELECT 'Categories' AS [Table], COUNT(*) AS [Count] FROM [dbo].[Categories]
UNION ALL
SELECT 'Users', COUNT(*) FROM [dbo].[Users]
UNION ALL
SELECT 'Applications', COUNT(*) FROM [dbo].[Applications];
GO

PRINT '';
PRINT 'Admin Login Credentials:';
PRINT '  Username: admin';
PRINT '  Password: admin123';
PRINT '';
GO
