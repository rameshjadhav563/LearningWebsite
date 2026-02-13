-- ============================================
-- COMPLETE DATABASE CLEANUP SCRIPT
-- Deletes ALL assessment data for fresh testing
-- ============================================

-- WARNING: This will delete ALL assessment data!
-- Make sure you have a backup if needed!

PRINT '🗑️ Starting database cleanup...';
GO

-- Step 1: Delete Assessment Answer Details
PRINT '1. Deleting assessment answer details...';
DELETE FROM AssessmentAnswerDetails;
PRINT '✅ AssessmentAnswerDetails cleared';
GO

-- Step 2: Delete Assessment Results
PRINT '2. Deleting assessment results...';
DELETE FROM AssessmentResults;
PRINT '✅ AssessmentResults cleared';
GO

-- Step 3: Reset Learning Assignments (set back to Not Started)
PRINT '3. Resetting learning assignments...';
UPDATE LearningAssignments
SET 
    Status = 'Not Started',
    ProgressPercentage = 0,
    CompletedDate = NULL
WHERE Status = 'Completed';
PRINT '✅ Learning assignments reset';
GO

-- Step 4: Verify cleanup
PRINT '';
PRINT '📊 Verification - Current record counts:';
SELECT 'AssessmentAnswerDetails' AS TableName, COUNT(*) AS RecordCount FROM AssessmentAnswerDetails
UNION ALL
SELECT 'AssessmentResults', COUNT(*) FROM AssessmentResults
UNION ALL
SELECT 'Completed Assignments', COUNT(*) FROM LearningAssignments WHERE Status = 'Completed';
GO

PRINT '';
PRINT '✅ Database cleanup complete!';
PRINT '🚀 Ready for fresh testing!';
GO
