-- Adds LinkedHashtagMessage column to Courses table if it does not already exist
USE [HrCertificatePortalDb];
GO

IF COL_LENGTH('dbo.Courses', 'LinkedHashtagMessage') IS NULL
BEGIN
    ALTER TABLE [dbo].[Courses]
    ADD [LinkedHashtagMessage] NVARCHAR(2048) NULL;
END
GO
