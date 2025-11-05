-- Adds CertificateUrl column to Courses table if it does not already exist
USE [HrCertificatePortalDb];
GO

IF COL_LENGTH('dbo.Courses', 'CertificateUrl') IS NULL
BEGIN
    ALTER TABLE [dbo].[Courses]
    ADD [CertificateUrl] NVARCHAR(2048) NULL;
END
GO
