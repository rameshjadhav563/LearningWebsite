using LearningWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningWebsite.Data
{
    public static class LearningDataInitializer
    {
        /// <summary>
        /// Seed sample learning data for testing dashboards
        /// Call this from DbInitializer.Initialize() or a separate initialization method
        /// </summary>
        public static void InitializeLearnings(AppDbContext context, IPasswordHasher<ApplicationUser> hasher)
        {
            // Add sample learnings if they don't exist
            if (!context.Learnings.Any())
            {
                var learnings = new[]
                {
                    new Learning
                    {
                        Title = "C# Fundamentals",
                        Description = "Learn the basics of C# programming language",
                        Category = "Technical",
                        DurationInHours = 30
                    },
                    new Learning
                    {
                        Title = "Advanced C# Concepts",
                        Description = "Deep dive into advanced C# features and patterns",
                        Category = "Technical",
                        DurationInHours = 40
                    },
                    new Learning
                    {
                        Title = "ASP.NET Core Fundamentals",
                        Description = "Introduction to ASP.NET Core web development",
                        Category = "Technical",
                        DurationInHours = 35
                    },
                    new Learning
                    {
                        Title = "Entity Framework Core",
                        Description = "Database access with Entity Framework Core",
                        Category = "Technical",
                        DurationInHours = 25
                    },
                    new Learning
                    {
                        Title = "Leadership Skills",
                        Description = "Develop effective leadership and management skills",
                        Category = "Soft Skills",
                        DurationInHours = 20
                    },
                    new Learning
                    {
                        Title = "Communication Excellence",
                        Description = "Enhance written and verbal communication skills",
                        Category = "Soft Skills",
                        DurationInHours = 15
                    },
                    new Learning
                    {
                        Title = "Project Management Basics",
                        Description = "Fundamentals of project management methodologies",
                        Category = "Professional Development",
                        DurationInHours = 30
                    },
                    new Learning
                    {
                        Title = "Cloud Computing Essentials",
                        Description = "Introduction to cloud platforms and services",
                        Category = "Technical",
                        DurationInHours = 28
                    }
                };

                context.Learnings.AddRange(learnings);
                context.SaveChanges();
            }

            // Add sample users with manager-employee relationships only if no users exist
            // (DbInitializer already creates initial users)
            var existingUsers = context.Users.ToList();

            if (existingUsers.Count <= 3) // Only initial users exist
            {
                // Create additional managers
                var manager2 = new ApplicationUser 
                { 
                    UserName = "mgr2", 
                    PasswordHash = hasher.HashPassword(null!, "Password123!"), 
                    Role = "Manager",
                    FullName = "Jane Manager",
                    Email = "mgr2@company.com"
                };

                context.Users.Add(manager2);
                context.SaveChanges();

                // Get existing manager (mgr1)
                var manager1 = context.Users.FirstOrDefault(u => u.UserName == "mgr1");
                var savedManager2 = context.Users.FirstOrDefault(u => u.UserName == "mgr2");

                // Create additional employees assigned to managers
                var employees = new[]
                {
                    new ApplicationUser 
                    { 
                        UserName = "emp2", 
                        PasswordHash = hasher.HashPassword(null!, "Password123!"), 
                        Role = "Employee",
                        FullName = "Bob Employee",
                        Email = "emp2@company.com",
                        ManagerId = manager1?.Id
                    },
                    new ApplicationUser 
                    { 
                        UserName = "emp3", 
                        PasswordHash = hasher.HashPassword(null!, "Password123!"), 
                        Role = "Employee",
                        FullName = "Charlie Employee",
                        Email = "emp3@company.com",
                        ManagerId = manager1?.Id
                    },
                    new ApplicationUser 
                    { 
                        UserName = "emp4", 
                        PasswordHash = hasher.HashPassword(null!, "Password123!"), 
                        Role = "Employee",
                        FullName = "Diana Employee",
                        Email = "emp4@company.com",
                        ManagerId = savedManager2?.Id
                    },
                    new ApplicationUser 
                    { 
                        UserName = "emp5", 
                        PasswordHash = hasher.HashPassword(null!, "Password123!"), 
                        Role = "Employee",
                        FullName = "Eve Employee",
                        Email = "emp5@company.com",
                        ManagerId = savedManager2?.Id
                    }
                };

                context.Users.AddRange(employees);

                // Update emp1 with manager and full details
                var emp1 = context.Users.FirstOrDefault(u => u.UserName == "emp1");
                if (emp1 != null)
                {
                    emp1.FullName = "Alice Employee";
                    emp1.Email = "emp1@company.com";
                    emp1.ManagerId = manager1?.Id;
                }

                // Update mgr1 with full details
                if (manager1 != null)
                {
                    manager1.FullName = "John Manager";
                    manager1.Email = "mgr1@company.com";
                }

                // Update hr1 with full details
                var hr1 = context.Users.FirstOrDefault(u => u.UserName == "hr1");
                if (hr1 != null)
                {
                    hr1.FullName = "HR Admin";
                    hr1.Email = "hr1@company.com";
                }

                context.SaveChanges();
            }

            // Add sample assignments if they don't exist
            if (!context.LearningAssignments.Any())
            {
                var learnings = context.Learnings.ToList();
                var employees = context.Users.Where(u => u.Role == "Employee").ToList();

                var assignments = new List<LearningAssignment>();
                var random = new Random(42); // Fixed seed for consistent results

                foreach (var employee in employees)
                {
                    // Assign 4-6 random learnings to each employee
                    var assignmentCount = random.Next(4, 7);
                    var selectedLearnings = learnings.OrderBy(x => random.Next()).Take(assignmentCount);

                    foreach (var learning in selectedLearnings)
                    {
                        var status = random.Next(0, 3);
                        var statusString = status switch
                        {
                            0 => "Completed",
                            1 => "InProgress",
                            _ => "NotStarted"
                        };

                        var assignedDate = DateTime.Now.AddDays(random.Next(-30, -5));
                        var dueDate = assignedDate.AddDays(random.Next(20, 60));
                        var progressPercentage = statusString switch
                        {
                            "Completed" => 100,
                            "InProgress" => random.Next(20, 99),
                            _ => 0
                        };

                        assignments.Add(new LearningAssignment
                        {
                            UserId = employee.Id,
                            LearningId = learning.Id,
                            AssignedDate = assignedDate,
                            DueDate = dueDate,
                            Status = statusString,
                            ProgressPercentage = progressPercentage,
                            CompletedDate = statusString == "Completed" ? dueDate.AddDays(random.Next(-5, 0)) : null
                        });
                    }
                }

                context.LearningAssignments.AddRange(assignments);
                context.SaveChanges();
            }
        }
    }
}
