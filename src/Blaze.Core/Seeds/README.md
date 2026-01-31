# Blaze Database Seeds

This folder contains SQL seed scripts to populate the database with initial data.

## Prerequisites

Before running seeds, ensure:
1. SQL Server is running on `localhost`
2. Database `BlazeDb` exists (run migrations first)
3. Migration `003_AddUserIsAdmin.sql` has been applied

## Seed Files

| File | Description |
|------|-------------|
| `001_Categories.sql` | 10 application categories |
| `002_AdminUser.sql` | Admin user account |
| `003_SampleApplications.sql` | 15 sample applications |
| `RunAllSeeds.sql` | Master script to run all seeds |

## Running Seeds

### Option 1: Run All Seeds (Recommended)

Using SQLCMD:
```bash
cd src/Blaze.Core/Seeds
sqlcmd -S localhost -d BlazeDb -i "RunAllSeeds.sql"
```

### Option 2: Run Individual Seeds

Run each file in order using SQL Server Management Studio (SSMS) or SQLCMD:

```bash
sqlcmd -S localhost -d BlazeDb -i "001_Categories.sql"
sqlcmd -S localhost -d BlazeDb -i "002_AdminUser.sql"
sqlcmd -S localhost -d BlazeDb -i "003_SampleApplications.sql"
```

### Option 3: Using SSMS

1. Open SQL Server Management Studio
2. Connect to `localhost`
3. Open each `.sql` file
4. Execute in order (001, 002, 003)

## Admin Credentials

After running seeds, use these credentials to test the backoffice:

| Field | Value |
|-------|-------|
| Username | `admin` |
| Password | `admin123` |

## Seed Data Summary

### Categories (10)
- Productivity
- Development
- Graphics & Design
- Multimedia
- Utilities
- Internet
- Security
- Education
- Communication
- Business

### Sample Applications (15)
Popular open-source applications across all categories:
- Notepad++, LibreOffice (Productivity)
- VS Code, Git, Node.js (Development)
- GIMP, Inkscape (Graphics)
- VLC, Audacity (Multimedia)
- 7-Zip, Everything (Utilities)
- Firefox, Thunderbird (Internet)
- KeePass (Security)
- Signal (Communication)

## Re-running Seeds

Seeds use `MERGE` statements and are idempotent - safe to run multiple times. Existing records will be updated, new records will be inserted.
