using LearningWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LearningWebsite.Data
{
    public static class DbInitializer
    {
            public static void Initialize(AppDbContext context, IPasswordHasher<ApplicationUser> hasher)
                {
                    // Create the database & tables for the AppDbContext model if they don't exist.
                    context.Database.EnsureCreated();

                    // Ensure the simple Users table exists (useful when the DB already exists but without this table)
                    var ensureUsersTable = @"
        IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
        BEGIN
            CREATE TABLE [dbo].[Users] (
                [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [UserName] NVARCHAR(256) NOT NULL,
                [PasswordHash] NVARCHAR(MAX) NOT NULL,
                [Role] NVARCHAR(100) NULL,
                [FullName] NVARCHAR(256) NULL,
                [Email] NVARCHAR(256) NULL,
                [ManagerId] INT NULL,
                CONSTRAINT FK_Users_Manager FOREIGN KEY ([ManagerId]) REFERENCES [Users]([Id])
            )
        END
        ";
                    context.Database.ExecuteSqlRaw(ensureUsersTable);

                    // Add missing columns if table already exists
                    var addMissingColumns = @"
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'FullName')
        BEGIN
            ALTER TABLE [dbo].[Users] ADD [FullName] NVARCHAR(256) NULL
        END

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Email')
        BEGIN
            ALTER TABLE [dbo].[Users] ADD [Email] NVARCHAR(256) NULL
        END

        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'ManagerId')
        BEGIN
            ALTER TABLE [dbo].[Users] ADD [ManagerId] INT NULL
            ALTER TABLE [dbo].[Users] ADD CONSTRAINT FK_Users_Manager FOREIGN KEY ([ManagerId]) REFERENCES [Users]([Id])
        END
        ";
                    context.Database.ExecuteSqlRaw(addMissingColumns);

                    // Idempotent seeding: add each user only if that username is not already present.
                    var seeded = new List<(string UserName, string Role)>
                    {
                        ("emp1", "Employee"),
                        ("mgr1", "Manager"),
                        ("hr1", "HR")
                    };

                    foreach (var (userName, role) in seeded)
                    {
                        if (!context.Users.Any(u => u.UserName == userName))
                        {
                            var user = new ApplicationUser { UserName = userName, Role = role };
                            user.PasswordHash = hasher.HashPassword(user, "Password123!");
                            context.Users.Add(user);
                        }
                    }

                    context.SaveChanges();

                    // Initialize learning data (learnings and assignments)
                    LearningDataInitializer.InitializeLearnings(context, hasher);
                }
    }
}