using LearningWebsite.Controllers;
using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace LearningWebsite.Tests.Controllers;

public class AssessmentControllerCertificateTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AssessmentController _controller;
    private readonly ApplicationUser _testUser;
    private readonly Learning _testLearning;

    public AssessmentControllerCertificateTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // Setup test user
        _testUser = new ApplicationUser
        {
            Id = 1,
            UserName = "test@example.com",
            Email = "test@example.com",
            FullName = "Test User",
            Role = "Employee"
        };

        // Setup test learning
        _testLearning = new Learning
        {
            Id = 1,
            Title = ".NET Full Stack Development - Beginner",
            Description = "Test course",
            Category = "Technical - Certification",
            DurationInHours = 40
        };

        _context.Users.Add(_testUser);
        _context.Learnings.Add(_testLearning);
        _context.SaveChanges();

        var loggerMock = new Mock<ILogger<AssessmentController>>();
        _controller = new AssessmentController(_context, loggerMock.Object);
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
    public async Task SubmitAssessment_GeneratesCertificate_WhenUserPasses()
    {
        // Arrange
        var assignment = new LearningAssignment
        {
            Id = 1,
            UserId = _testUser.Id,
            LearningId = _testLearning.Id,
            AssignedDate = DateTime.Now.AddDays(-7),
            DueDate = DateTime.Now.AddDays(7),
            Status = "InProgress",
            ProgressPercentage = 50
        };

        var questions = new List<Question>
        {
            new() { Id = 1, LearningId = _testLearning.Id, QuestionText = "Q1", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "A", DifficultyLevel = "Beginner" },
            new() { Id = 2, LearningId = _testLearning.Id, QuestionText = "Q2", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "B", DifficultyLevel = "Beginner" },
            new() { Id = 3, LearningId = _testLearning.Id, QuestionText = "Q3", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "C", DifficultyLevel = "Beginner" },
            new() { Id = 4, LearningId = _testLearning.Id, QuestionText = "Q4", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "D", DifficultyLevel = "Beginner" },
            new() { Id = 5, LearningId = _testLearning.Id, QuestionText = "Q5", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "A", DifficultyLevel = "Beginner" },
            new() { Id = 6, LearningId = _testLearning.Id, QuestionText = "Q6", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "B", DifficultyLevel = "Beginner" },
            new() { Id = 7, LearningId = _testLearning.Id, QuestionText = "Q7", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "C", DifficultyLevel = "Beginner" },
            new() { Id = 8, LearningId = _testLearning.Id, QuestionText = "Q8", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "D", DifficultyLevel = "Beginner" },
            new() { Id = 9, LearningId = _testLearning.Id, QuestionText = "Q9", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "A", DifficultyLevel = "Beginner" },
            new() { Id = 10, LearningId = _testLearning.Id, QuestionText = "Q10", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "B", DifficultyLevel = "Beginner" }
        };

        _context.LearningAssignments.Add(assignment);
        _context.Questions.AddRange(questions);
        await _context.SaveChangesAsync();

        var submission = new AssessmentSubmissionModel
        {
            LearningId = _testLearning.Id,
            Answers = new Dictionary<int, string>
            {
                { 1, "A" }, // Correct
                { 2, "B" }, // Correct
                { 3, "C" }, // Correct
                { 4, "D" }, // Correct
                { 5, "A" }, // Correct
                { 6, "B" }, // Correct
                { 7, "C" }, // Correct
                { 8, "D" }, // Correct
                { 9, "A" }, // Correct
                { 10, "A" } // Wrong - 90% score
            }
        };

        // Act
        var result = await _controller.SubmitAssessment(submission);

        // Assert
        Assert.IsType<ViewResult>(result);

        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == _testUser.Id && c.LearningId == _testLearning.Id);

        Assert.NotNull(certificate);
        Assert.Equal(_testUser.Id, certificate.UserId);
        Assert.Equal(_testLearning.Id, certificate.LearningId);
        Assert.Equal("Beginner", certificate.DifficultyLevel);
        Assert.Equal(90, certificate.Score);
        Assert.Matches(@"^CERT-\d{8}-\d{4}-\d{4}$", certificate.CertificateNumber);
    }

    [Fact]
    public async Task SubmitAssessment_DoesNotGenerateCertificate_WhenUserFails()
    {
        // Arrange
        var assignment = new LearningAssignment
        {
            Id = 1,
            UserId = _testUser.Id,
            LearningId = _testLearning.Id,
            AssignedDate = DateTime.Now.AddDays(-7),
            DueDate = DateTime.Now.AddDays(7),
            Status = "InProgress",
            ProgressPercentage = 50
        };

        var questions = new List<Question>
        {
            new() { Id = 1, LearningId = _testLearning.Id, QuestionText = "Q1", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "A", DifficultyLevel = "Beginner" },
            new() { Id = 2, LearningId = _testLearning.Id, QuestionText = "Q2", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "B", DifficultyLevel = "Beginner" },
            new() { Id = 3, LearningId = _testLearning.Id, QuestionText = "Q3", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "C", DifficultyLevel = "Beginner" },
            new() { Id = 4, LearningId = _testLearning.Id, QuestionText = "Q4", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "D", DifficultyLevel = "Beginner" },
            new() { Id = 5, LearningId = _testLearning.Id, QuestionText = "Q5", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "A", DifficultyLevel = "Beginner" },
            new() { Id = 6, LearningId = _testLearning.Id, QuestionText = "Q6", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "B", DifficultyLevel = "Beginner" },
            new() { Id = 7, LearningId = _testLearning.Id, QuestionText = "Q7", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "C", DifficultyLevel = "Beginner" },
            new() { Id = 8, LearningId = _testLearning.Id, QuestionText = "Q8", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "D", DifficultyLevel = "Beginner" },
            new() { Id = 9, LearningId = _testLearning.Id, QuestionText = "Q9", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "A", DifficultyLevel = "Beginner" },
            new() { Id = 10, LearningId = _testLearning.Id, QuestionText = "Q10", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "B", DifficultyLevel = "Beginner" }
        };

        _context.LearningAssignments.Add(assignment);
        _context.Questions.AddRange(questions);
        await _context.SaveChangesAsync();

        var submission = new AssessmentSubmissionModel
        {
            LearningId = _testLearning.Id,
            Answers = new Dictionary<int, string>
            {
                { 1, "A" }, // Correct
                { 2, "B" }, // Correct
                { 3, "C" }, // Correct
                { 4, "D" }, // Correct
                { 5, "A" }, // Correct
                { 6, "A" }, // Wrong
                { 7, "A" }, // Wrong
                { 8, "A" }, // Wrong
                { 9, "A" }, // Correct
                { 10, "A" } // Wrong - 60% score (fails)
            }
        };

        // Act
        var result = await _controller.SubmitAssessment(submission);

        // Assert
        Assert.IsType<ViewResult>(result);

        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == _testUser.Id && c.LearningId == _testLearning.Id);

        Assert.Null(certificate); // No certificate should be generated
    }

    [Fact]
    public async Task SubmitAssessment_StoresDifficultyLevel_InAssessmentResult()
    {
        // Arrange
        var assignment = new LearningAssignment
        {
            Id = 1,
            UserId = _testUser.Id,
            LearningId = _testLearning.Id,
            AssignedDate = DateTime.Now.AddDays(-7),
            DueDate = DateTime.Now.AddDays(7),
            Status = "InProgress",
            ProgressPercentage = 50
        };

        var questions = new List<Question>
        {
            new() { Id = 1, LearningId = _testLearning.Id, QuestionText = "Q1", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "A", DifficultyLevel = "Intermediate" },
            new() { Id = 2, LearningId = _testLearning.Id, QuestionText = "Q2", OptionA = "A", OptionB = "B", OptionC = "C", OptionD = "D", CorrectAnswer = "B", DifficultyLevel = "Intermediate" }
        };

        _context.LearningAssignments.Add(assignment);
        _context.Questions.AddRange(questions);
        await _context.SaveChangesAsync();

        var submission = new AssessmentSubmissionModel
        {
            LearningId = _testLearning.Id,
            Answers = new Dictionary<int, string>
            {
                { 1, "A" }, // Correct
                { 2, "B" }  // Correct - 100% score
            }
        };

        // Act
        await _controller.SubmitAssessment(submission);

        // Assert
        var assessmentResult = await _context.AssessmentResults
            .FirstOrDefaultAsync(ar => ar.UserId == _testUser.Id && ar.LearningId == _testLearning.Id);

        Assert.NotNull(assessmentResult);
        Assert.Equal("Intermediate", assessmentResult.DifficultyLevel);
    }

    [Theory]
    [InlineData(70.0)]  // Minimum passing score
    [InlineData(85.0)]  // Above minimum
    [InlineData(100.0)] // Perfect score
    public async Task SubmitAssessment_GeneratesCertificate_ForAllPassingScores(decimal expectedScore)
    {
        // Arrange
        var assignment = new LearningAssignment
        {
            Id = 1,
            UserId = _testUser.Id,
            LearningId = _testLearning.Id,
            AssignedDate = DateTime.Now.AddDays(-7),
            DueDate = DateTime.Now.AddDays(7),
            Status = "InProgress",
            ProgressPercentage = 50
        };

        var questionCount = 10;
        var correctAnswers = (int)Math.Ceiling(questionCount * (expectedScore / 100));

        var questions = new List<Question>();
        for (int i = 1; i <= questionCount; i++)
        {
            questions.Add(new Question
            {
                Id = i,
                LearningId = _testLearning.Id,
                QuestionText = $"Q{i}",
                OptionA = "A",
                OptionB = "B",
                OptionC = "C",
                OptionD = "D",
                CorrectAnswer = "A",
                DifficultyLevel = "Beginner"
            });
        }

        _context.LearningAssignments.Add(assignment);
        _context.Questions.AddRange(questions);
        await _context.SaveChangesAsync();

        var answers = new Dictionary<int, string>();
        for (int i = 1; i <= questionCount; i++)
        {
            answers[i] = i <= correctAnswers ? "A" : "B"; // Correct answers first, then wrong
        }

        var submission = new AssessmentSubmissionModel
        {
            LearningId = _testLearning.Id,
            Answers = answers
        };

        // Act
        await _controller.SubmitAssessment(submission);

        // Assert
        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == _testUser.Id && c.LearningId == _testLearning.Id);

        Assert.NotNull(certificate);
        Assert.True(certificate.Score >= 70);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
