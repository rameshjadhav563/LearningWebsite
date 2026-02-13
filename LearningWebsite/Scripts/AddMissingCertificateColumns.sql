-- Script to add missing columns to Certificates table

-- Check and add Description column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Certificates]') AND name = 'Description')
BEGIN
    ALTER TABLE [dbo].[Certificates] ADD [Description] NVARCHAR(500) NULL
    PRINT 'Added Description column'
END
ELSE
BEGIN
    PRINT 'Description column already exists'
END

-- Check and add EmployeeName column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Certificates]') AND name = 'EmployeeName')
BEGIN
    ALTER TABLE [dbo].[Certificates] ADD [EmployeeName] NVARCHAR(100) NOT NULL DEFAULT('')
    PRINT 'Added EmployeeName column'
END
ELSE
BEGIN
    PRINT 'EmployeeName column already exists'
END

-- Check and add LearningTitle column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Certificates]') AND name = 'LearningTitle')
BEGIN
    ALTER TABLE [dbo].[Certificates] ADD [LearningTitle] NVARCHAR(200) NOT NULL DEFAULT('')
    PRINT 'Added LearningTitle column'
END
ELSE
BEGIN
    PRINT 'LearningTitle column already exists'
END

PRINT 'Certificate table schema update complete!'
