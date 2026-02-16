using LearningWebsite.Controllers;
using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace LearningWebsite.Tests.Controllers
{
    public class ManagerControllerTests : IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Mock<IPasswordHasher<ApplicationUser>> _passwordHasherMock;
        private readonly Mock<ILogger<ManagerController>> _loggerMock;
        private readonly ManagerController _controller;

        public ManagerControllerTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new AppDbContext(options);
            _passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
            _loggerMock = new Mock<ILogger<ManagerController>>();

            _controller = new ManagerController(_context, _passwordHasherMock.Object, _loggerMock.Object);

            SetupTestData();
            SetupControllerContext();
        }

        private void SetupTestData()
        {
            var manager = new ApplicationUser
            {
                Id = 1,
                UserName = "manager1",
                FullName = "Manager One",
                Email = "manager1@test.com",
                Role = "Manager"
            };

            var employee1 = new ApplicationUser
            {
                Id = 2,
                UserName = "employee1",
                FullName = "Employee One",
                Email = "employee1@test.com",
                Role = "Employee",
                ManagerId = 1
            };

            var employee2 = new ApplicationUser
            {
                Id = 3,
                UserName = "employee2",
                FullName = "Employee Two",
                Email = "employee2@test.com",
                Role = "Employee",
                ManagerId = 1
            };

            var learning1 = new Learning
            {
                Id = 1,
                Title = "C# Basics",
                Category = "Programming",
                Description = "Learn C#",
                DurationInHours = 60
            };

            var learning2 = new Learning
            {
                Id = 2,
                Title = "ASP.NET Core",
                Category = "Web Development",
                Description = "Learn ASP.NET Core",
                DurationInHours = 120
            };

            _context.Users.AddRange(manager, employee1, employee2);
            _context.Learnings.AddRange(learning1, learning2);
            _context.SaveChanges();

            var assignments = new[]
            {
                new LearningAssignment
                {
                    Id = 1,
                    UserId = 2,
                    LearningId = 1,
                    AssignedDate = DateTime.Now.AddDays(-10),
                    DueDate = DateTime.Now.AddDays(20),
                    Status = "Completed",
                    ProgressPercentage = 100,
                    CompletedDate = DateTime.Now.AddDays(-2)
                },
                new LearningAssignment
                {
                    Id = 2,
                    UserId = 2,
                    LearningId = 2,
                    AssignedDate = DateTime.Now.AddDays(-5),
                    DueDate = DateTime.Now.AddDays(25),
                    Status = "InProgress",
                    ProgressPercentage = 50
                },
                new LearningAssignment
                {
                    Id = 3,
                    UserId = 3,
                    LearningId = 1,
                    AssignedDate = DateTime.Now.AddDays(-30),
                    DueDate = DateTime.Now.AddDays(-5),
                    Status = "NotStarted",
                    ProgressPercentage = 0
                },
                new LearningAssignment
                {
                    Id = 4,
                    UserId = 3,
                    LearningId = 2,
                    AssignedDate = DateTime.Now.AddDays(-8),
                    DueDate = DateTime.Now.AddDays(22),
                    Status = "Completed",
                    ProgressPercentage = 100,
                    CompletedDate = DateTime.Now.AddDays(-1)
                }
            };

            _context.LearningAssignments.AddRange(assignments);
            _context.SaveChanges();
        }

        private void SetupControllerContext()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "manager1"),
                new Claim(ClaimTypes.Role, "Manager")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        [Fact]
        public void TeamMetrics_ReturnsViewResult_WithCorrectViewModel()
        {
            var result = _controller.TeamMetrics();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<TeamMetricsViewModel>(viewResult.Model);
        }

        [Fact]
        public void TeamMetrics_CalculatesCorrectTotalTeamMembers()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(2, model.TotalTeamMembers);
        }

        [Fact]
        public void TeamMetrics_CalculatesCorrectTotalAssignments()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(4, model.TotalAssignments);
        }

        [Fact]
        public void TeamMetrics_CalculatesCorrectCompletedAssignments()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(2, model.CompletedAssignments);
        }

        [Fact]
        public void TeamMetrics_CalculatesCorrectInProgressAssignments()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(1, model.InProgressAssignments);
        }

        [Fact]
        public void TeamMetrics_CalculatesCorrectNotStartedAssignments()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(1, model.NotStartedAssignments);
        }

        [Fact]
        public void TeamMetrics_CalculatesCorrectOverdueAssignments()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(1, model.OverdueAssignments);
        }

        [Fact]
        public void TeamMetrics_CalculatesCorrectTeamCompletionRate()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(50.0, model.TeamCompletionRate);
        }

        [Fact]
        public void TeamMetrics_PopulatesTeamMemberMetrics()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(2, model.TeamMemberMetrics.Count);
        }

        [Fact]
        public void TeamMetrics_TeamMemberMetrics_ContainsCorrectData()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);

            var employee1Metrics = model.TeamMemberMetrics.FirstOrDefault(m => m.UserName == "employee1");
            Assert.NotNull(employee1Metrics);
            Assert.Equal(2, employee1Metrics.TotalAssignments);
            Assert.Equal(1, employee1Metrics.CompletedAssignments);
            Assert.Equal(1, employee1Metrics.InProgressAssignments);
            Assert.Equal(0, employee1Metrics.NotStartedAssignments);
            Assert.Equal(50.0, employee1Metrics.CompletionRate);
        }

        [Fact]
        public void TeamMetrics_PopulatesCategoryMetrics()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(2, model.CategoryMetrics.Count);
        }

        [Fact]
        public void TeamMetrics_CategoryMetrics_ContainsCorrectCompletionRates()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);

            var programmingCategory = model.CategoryMetrics.FirstOrDefault(c => c.Category == "Programming");
            Assert.NotNull(programmingCategory);
            Assert.Equal(2, programmingCategory.TotalAssignments);
            Assert.Equal(1, programmingCategory.CompletedAssignments);
            Assert.Equal(50.0, programmingCategory.CompletionRate);
        }

        [Fact]
        public void TeamMetrics_PopulatesRecentActivities()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.NotEmpty(model.RecentActivities);
            Assert.True(model.RecentActivities.Count <= 10);
        }

        [Fact]
        public void TeamMetrics_RecentActivities_OrderedByDateDescending()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.NotEmpty(model.RecentActivities);

            for (int i = 0; i < model.RecentActivities.Count - 1; i++)
            {
                Assert.True(model.RecentActivities[i].ActivityDate >= model.RecentActivities[i + 1].ActivityDate);
            }
        }

        [Fact]
        public void TeamMetrics_ReturnsNotFound_WhenManagerNotFound()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "nonexistent"),
                new Claim(ClaimTypes.Role, "Manager")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            var result = _controller.TeamMetrics();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void TeamMetrics_HandlesEmptyTeam()
        {
            var newManager = new ApplicationUser
            {
                Id = 10,
                UserName = "manager2",
                FullName = "Manager Two",
                Email = "manager2@test.com",
                Role = "Manager"
            };

            _context.Users.Add(newManager);
            _context.SaveChanges();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "manager2"),
                new Claim(ClaimTypes.Role, "Manager")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };

            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            Assert.Equal(0, model.TotalTeamMembers);
            Assert.Equal(0, model.TotalAssignments);
            Assert.Equal(0, model.TeamCompletionRate);
        }

        [Fact]
        public void TeamMetrics_CalculatesAverageProgressPercentage()
        {
            var result = _controller.TeamMetrics() as ViewResult;
            var model = result?.Model as TeamMetricsViewModel;

            Assert.NotNull(model);
            var expectedAverage = (100.0 + 50.0 + 0.0 + 100.0) / 4;
            Assert.Equal(Math.Round(expectedAverage, 2), model.AverageProgressPercentage);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
