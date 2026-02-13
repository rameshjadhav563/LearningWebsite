using Microsoft.EntityFrameworkCore;

namespace LearningWebsite.Data
{
    public static class DatabaseCleaner
    {
        /// <summary>
        /// Deletes all data from all tables in the database.
        /// Tables are cleared in order to respect foreign key constraints.
        /// </summary>
        public static void ClearAllData(AppDbContext context)
        {
            // Delete in order to respect foreign key constraints
            // Start with the most dependent tables first

            context.Database.ExecuteSqlRaw("DELETE FROM AssessmentAnswerDetails");
            context.Database.ExecuteSqlRaw("DELETE FROM Certificates");
            context.Database.ExecuteSqlRaw("DELETE FROM AssessmentResults");
            context.Database.ExecuteSqlRaw("DELETE FROM LearningAssignments");
            context.Database.ExecuteSqlRaw("DELETE FROM Questions");
            context.Database.ExecuteSqlRaw("DELETE FROM Learnings");
            context.Database.ExecuteSqlRaw("DELETE FROM Users");

            // Reset identity seeds (optional - uncomment if you want IDs to start from 1 again)
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('AssessmentAnswerDetails', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Certificates', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('AssessmentResults', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('LearningAssignments', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Questions', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Learnings', RESEED, 0)");
            // context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Users', RESEED, 0)");
        }

        /// <summary>
        /// Deletes all data and re-seeds the database with initial data.
        /// </summary>
        public static void ResetDatabase(AppDbContext context, Microsoft.AspNetCore.Identity.IPasswordHasher<Models.ApplicationUser> hasher)
        {
            ClearAllData(context);
            DbInitializer.Initialize(context, hasher);
            QuestionDataInitializer.Initialize(context);
        }
    }
}
