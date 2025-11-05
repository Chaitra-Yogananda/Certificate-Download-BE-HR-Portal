-- Initial SQL Server schema for HR Certificate Portal
-- This script creates the database (if missing), tables, constraints, and indexes

IF DB_ID(N'HrCertificatePortalDb') IS NULL
BEGIN
    PRINT 'Creating database HrCertificatePortalDb...';
    EXEC('CREATE DATABASE [HrCertificatePortalDb]');
END
GO

USE [HrCertificatePortalDb];
GO

-- Users table
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Users]
    (
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Users] PRIMARY KEY,
        [Email] NVARCHAR(450) NOT NULL,
        [PasswordHash] NVARCHAR(MAX) NOT NULL,
        [Role] NVARCHAR(100) NOT NULL,
        [CreatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Users_CreatedDate] DEFAULT SYSUTCDATETIME(),
        [CreatedBy] NVARCHAR(MAX) NULL,
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Users_IsActive] DEFAULT 1,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_Users_IsDeleted] DEFAULT 0
    );
    CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
END
GO

-- Courses table
IF OBJECT_ID(N'[dbo].[Courses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Courses]
    (
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Courses] PRIMARY KEY,
        [CourseName] NVARCHAR(200) NOT NULL,
        [CourseCode] NVARCHAR(50) NOT NULL,
        [Template] VARBINARY(MAX) NULL,
        [CertificateUrl] NVARCHAR(2048) NULL,
        [CreatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Courses_CreatedDate] DEFAULT SYSUTCDATETIME(),
        [CreatedBy] NVARCHAR(MAX) NULL,
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Courses_IsActive] DEFAULT 1,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_Courses_IsDeleted] DEFAULT 0
    );
END
GO

-- Employees table
IF OBJECT_ID(N'[dbo].[Employees]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Employees]
    (
        [Id] INT IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Employees] PRIMARY KEY,
        [CourseId] INT NOT NULL,
        [EmployeeName] NVARCHAR(200) NOT NULL,
        [EmployeeEmail] NVARCHAR(200) NOT NULL,
        [Batch] NVARCHAR(100) NULL,
        [CreatedDate] DATETIME2 NOT NULL CONSTRAINT [DF_Employees_CreatedDate] DEFAULT SYSUTCDATETIME(),
        [CreatedBy] NVARCHAR(MAX) NULL,
        [ModifiedDate] DATETIME2 NULL,
        [ModifiedBy] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL CONSTRAINT [DF_Employees_IsActive] DEFAULT 1,
        [IsDeleted] BIT NOT NULL CONSTRAINT [DF_Employees_IsDeleted] DEFAULT 0
    );

    ALTER TABLE [dbo].[Employees]
    ADD CONSTRAINT [FK_Employees_Courses_CourseId]
        FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses]([Id]) ON DELETE CASCADE;

    CREATE INDEX [IX_Employees_CourseId] ON [dbo].[Employees]([CourseId]);
END
GO

-- Optional seed data is handled by the application at startup.
-- To manually insert a sample course, uncomment below:
-- INSERT INTO [dbo].[Courses] ([CourseName], [CourseCode]) VALUES (N'Sample Course', N'SC-001');
