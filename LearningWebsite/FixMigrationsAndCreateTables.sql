-- Fix Migration History and Create Assessment Tables
-- Run this script in SQL Server Management Studio or any SQL client

USE LearningWebsiteDb;
GO

-- Step 1: Check current migration history
SELECT * FROM __EFMigrationsHistory;
GO

-- Step 2: Add the first migration to history if missing (assuming tables already exist)
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260205064545_CreateLearningTables')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260205064545_CreateLearningTables', '8.0.0');
    PRINT 'Added CreateLearningTables migration to history';
END
ELSE
BEGIN
    PRINT 'CreateLearningTables migration already exists in history';
END
GO

-- Step 3: Create Questions table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Questions')
BEGIN
    CREATE TABLE [dbo].[Questions] (
        [Id] int NOT NULL IDENTITY(1,1),
        [LearningId] int NOT NULL,
        [QuestionText] nvarchar(max) NOT NULL,
        [OptionA] nvarchar(max) NOT NULL,
        [OptionB] nvarchar(max) NOT NULL,
        [OptionC] nvarchar(max) NOT NULL,
        [OptionD] nvarchar(max) NOT NULL,
        [CorrectAnswer] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Questions_Learnings_LearningId] FOREIGN KEY ([LearningId])
            REFERENCES [Learnings] ([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_Questions_LearningId] ON [Questions] ([LearningId]);
    PRINT 'Created Questions table';
END
ELSE
BEGIN
    PRINT 'Questions table already exists';
END
GO

-- Step 4: Create AssessmentResults table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AssessmentResults')
BEGIN
    CREATE TABLE [dbo].[AssessmentResults] (
        [Id] int NOT NULL IDENTITY(1,1),
        [UserId] int NOT NULL,
        [LearningId] int NOT NULL,
        [CompletedDate] datetime2 NOT NULL,
        [TotalQuestions] int NOT NULL,
        [CorrectAnswers] int NOT NULL,
        [Score] decimal(18,2) NOT NULL,
        [Passed] bit NOT NULL,
        CONSTRAINT [PK_AssessmentResults] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AssessmentResults_Learnings_LearningId] FOREIGN KEY ([LearningId])
            REFERENCES [Learnings] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AssessmentResults_Users_UserId] FOREIGN KEY ([UserId])
            REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
    
    CREATE INDEX [IX_AssessmentResults_LearningId] ON [AssessmentResults] ([LearningId]);
    CREATE INDEX [IX_AssessmentResults_UserId] ON [AssessmentResults] ([UserId]);
    PRINT 'Created AssessmentResults table';
END
ELSE
BEGIN
    PRINT 'AssessmentResults table already exists';
END
GO

-- Step 5: Add the new migration to history
IF NOT EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '20260206080050_AddAssessmentTables')
BEGIN
    INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
    VALUES ('20260206080050_AddAssessmentTables', '8.0.0');
    PRINT 'Added AddAssessmentTables migration to history';
END
ELSE
BEGIN
    PRINT 'AddAssessmentTables migration already exists in history';
END
GO

-- Step 6: Verify all tables exist
PRINT 'Verifying tables...';
SELECT name FROM sys.tables WHERE name IN ('Users', 'Learnings', 'LearningAssignments', 'Questions', 'AssessmentResults')
ORDER BY name;
GO

PRINT 'Setup complete! You can now run the application.';
