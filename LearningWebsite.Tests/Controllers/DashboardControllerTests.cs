using LearningWebsite.Controllers.Api;
using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Text.Json;

namespace LearningWebsite.Tests.Controllers;

public class DashboardControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly DashboardController _controller;
    private readonly ApplicationUser _testUser;

    public DashboardControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _testUser = new ApplicationUser
        {
            Id = 1,
            UserName = "test@example.com",
            Email = "test@example.com",
            FullName = "Test User",
            Role = "Employee"
        };

        _context.Users.Add(_testUser);
        _context.SaveChanges();

        var loggerMock = new Mock<ILogger<DashboardController>>();
        _controller = new DashboardController(_context, loggerMock.Object);
        SetupControllerContext();
    }

    private void SetupControllerContext()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _testUser.Email!),
            new Claim(ClaimTypes.NameIdentifier, _testUser.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetEmployeeDashboard_IncludesCertificatesCount()
    {
        // Arrange
        var learning = new Learning
        {
            Id = 1,
            Title = "Test Course",
            Category = "Technical",
            Description = "Test",
            DurationInHours = 10
        };

        var assignment = new LearningAssignment
        {
            Id = 1,
            UserId = _testUser.Id,
            LearningId = learning.Id,
            AssignedDate = DateTime.Now.AddDays(-7),
            DueDate = DateTime.Now.AddDays(7),
            Status = "Completed",
            ProgressPercentage = 100
        };

        var certificates = new List<Certificate>
        {
            new()
            {
                Id = 1,
                UserId = _testUser.Id,
                LearningId = learning.Id,
                AssessmentResultId = 1,
                CertificateNumber = "CERT-20260210-0001-0001",
                Title = "Test Course - Beginner",
                DifficultyLevel = "Beginner",
                IssuedDate = DateTime.Now,
                Score = 85
            },
            new()
            {
                Id = 2,
                UserId = _testUser.Id,
                LearningId = learning.Id,
                AssessmentResultId = 2,
                CertificateNumber = "CERT-20260210-0001-0002",
                Title = "Test Course - Intermediate",
                DifficultyLevel = "Intermediate",
                IssuedDate = DateTime.Now,
                Score = 90
            }
        };

        _context.Learnings.Add(learning);
        _context.LearningAssignments.Add(assignment);
        _context.Certificates.AddRange(certificates);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetEmployeeDashboard();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(okResult.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.Equal(2, data.GetProperty("certificatesCount").GetInt32());
    }

    [Fact]
    public async Task GetEmployeeDashboard_ReturnsZeroCertificates_WhenNoCertificatesExist()
    {
        // Act
        var result = await _controller.GetEmployeeDashboard();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(okResult.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.Equal(0, data.GetProperty("certificatesCount").GetInt32());
    }

    [Fact]
    public async Task GetEmployeeDashboard_ReturnsOnlyUsersCertificates()
    {
        // Arrange
        var otherUser = new ApplicationUser
        {
            Id = 2,
            UserName = "other@example.com",
            Email = "other@example.com",
            FullName = "Other User",
            Role = "Employee"
        };

        var learning = new Learning
        {
            Id = 1,
            Title = "Test Course",
            Category = "Technical",
            Description = "Test",
            DurationInHours = 10
        };

        var userCertificate = new Certificate
        {
            Id = 1,
            UserId = _testUser.Id,
            LearningId = learning.Id,
            AssessmentResultId = 1,
            CertificateNumber = "CERT-20260210-0001-0001",
            Title = "Test Course",
            DifficultyLevel = "Beginner",
            IssuedDate = DateTime.Now,
            Score = 85
        };

        var otherUserCertificate = new Certificate
        {
            Id = 2,
            UserId = otherUser.Id,
            LearningId = learning.Id,
            AssessmentResultId = 2,
            CertificateNumber = "CERT-20260210-0002-0001",
            Title = "Test Course",
            DifficultyLevel = "Beginner",
            IssuedDate = DateTime.Now,
            Score = 90
        };

        _context.Users.Add(otherUser);
        _context.Learnings.Add(learning);
        _context.Certificates.AddRange(userCertificate, otherUserCertificate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetEmployeeDashboard();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(okResult.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.Equal(1, data.GetProperty("certificatesCount").GetInt32());
    }

    [Fact]
    public async Task GetEmployeeDashboard_ReturnsCompleteData_WithAssignmentsAndCertificates()
    {
        // Arrange
        var learning = new Learning
        {
            Id = 1,
            Title = "Test Course",
            Category = "Technical",
            Description = "Test",
            DurationInHours = 10
        };

        var assignments = new List<LearningAssignment>
        {
            new()
            {
                Id = 1,
                UserId = _testUser.Id,
                LearningId = learning.Id,
                AssignedDate = DateTime.Now.AddDays(-10),
                DueDate = DateTime.Now.AddDays(5),
                Status = "Completed",
                ProgressPercentage = 100
            },
            new()
            {
                Id = 2,
                UserId = _testUser.Id,
                LearningId = learning.Id,
                AssignedDate = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(10),
                Status = "InProgress",
                ProgressPercentage = 50
            }
        };

        var certificate = new Certificate
        {
            Id = 1,
            UserId = _testUser.Id,
            LearningId = learning.Id,
            AssessmentResultId = 1,
            CertificateNumber = "CERT-20260210-0001-0001",
            Title = "Test Course",
            DifficultyLevel = "Beginner",
            IssuedDate = DateTime.Now,
            Score = 85
        };

        _context.Learnings.Add(learning);
        _context.LearningAssignments.AddRange(assignments);
        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetEmployeeDashboard();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var json = JsonSerializer.Serialize(okResult.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.Equal(2, data.GetProperty("totalAssignments").GetInt32());
        Assert.Equal(1, data.GetProperty("completed").GetInt32());
        Assert.Equal(1, data.GetProperty("inProgress").GetInt32());
        Assert.Equal(1, data.GetProperty("certificatesCount").GetInt32());
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
