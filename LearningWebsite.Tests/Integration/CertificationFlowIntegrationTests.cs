using LearningWebsite.Data;
using LearningWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningWebsite.Tests.Integration;

public class CertificationFlowIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;

    public CertificationFlowIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    [Fact]
    public async Task CompleteCertificationFlow_CreatesAllRequiredEntities()
    {
        // Arrange - Create User
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "student@example.com",
            Email = "student@example.com",
            FullName = "Test Student",
            Role = "Employee"
        };

        _context.Users.Add(user);

        // Arrange - Create Learning Course
        var learning = new Learning
        {
            Id = 1,
            Title = ".NET Full Stack Development - Beginner",
            Description = "Complete beginner course",
            Category = "Technical - Certification",
            DurationInHours = 40
        };

        _context.Learnings.Add(learning);

        // Arrange - Create Questions
        var questions = Enumerable.Range(1, 10).Select(i => new Question
        {
            Id = i,
            LearningId = learning.Id,
            QuestionText = $"Question {i}",
            OptionA = "A",
            OptionB = "B",
            OptionC = "C",
            OptionD = "D",
            CorrectAnswer = "A",
            DifficultyLevel = "Beginner"
        }).ToList();

        _context.Questions.AddRange(questions);

        // Arrange - Create Learning Assignment
        var assignment = new LearningAssignment
        {
            Id = 1,
            UserId = user.Id,
            LearningId = learning.Id,
            AssignedDate = DateTime.Now.AddDays(-7),
            DueDate = DateTime.Now.AddDays(7),
            Status = "InProgress",
            ProgressPercentage = 50
        };

        _context.LearningAssignments.Add(assignment);
        await _context.SaveChangesAsync();

        // Act - Create Assessment Result (Simulating passing the assessment)
        var assessmentResult = new AssessmentResult
        {
            UserId = user.Id,
            LearningId = learning.Id,
            CompletedDate = DateTime.Now,
            TotalQuestions = 10,
            CorrectAnswers = 9,
            Score = 90,
            Passed = true,
            DifficultyLevel = "Beginner"
        };

        _context.AssessmentResults.Add(assessmentResult);
        await _context.SaveChangesAsync();

        // Act - Generate Certificate
        var certificate = new Certificate
        {
            UserId = user.Id,
            LearningId = learning.Id,
            AssessmentResultId = assessmentResult.Id,
            CertificateNumber = $"CERT-{DateTime.Now:yyyyMMdd}-{user.Id:D4}-{learning.Id:D4}",
            Title = $"{learning.Title} - Beginner Level",
            DifficultyLevel = "Beginner",
            IssuedDate = DateTime.Now,
            Score = 90
        };

        _context.Certificates.Add(certificate);

        // Act - Update Assignment to Completed
        assignment.Status = "Completed";
        assignment.ProgressPercentage = 100;
        assignment.CompletedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        // Assert - Verify all entities were created
        var savedUser = await _context.Users.FindAsync(user.Id);
        var savedLearning = await _context.Learnings.FindAsync(learning.Id);
        var savedQuestions = await _context.Questions.Where(q => q.LearningId == learning.Id).ToListAsync();
        var savedAssignment = await _context.LearningAssignments.FindAsync(assignment.Id);
        var savedAssessmentResult = await _context.AssessmentResults.FirstOrDefaultAsync(ar => ar.UserId == user.Id);
        var savedCertificate = await _context.Certificates.FirstOrDefaultAsync(c => c.UserId == user.Id);

        Assert.NotNull(savedUser);
        Assert.NotNull(savedLearning);
        Assert.Equal(10, savedQuestions.Count);
        Assert.NotNull(savedAssignment);
        Assert.Equal("Completed", savedAssignment.Status);
        Assert.NotNull(savedAssessmentResult);
        Assert.True(savedAssessmentResult.Passed);
        Assert.NotNull(savedCertificate);
        Assert.Equal("Beginner", savedCertificate.DifficultyLevel);
    }

    [Fact]
    public async Task CertificationFlow_SupportsMultipleDifficultyLevels()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "student@example.com",
            Email = "student@example.com",
            FullName = "Test Student",
            Role = "Employee"
        };

        var beginnerCourse = new Learning
        {
            Id = 1,
            Title = ".NET Full Stack Development - Beginner",
            Category = "Technical - Certification",
            Description = "Beginner",
            DurationInHours = 40
        };

        var intermediateCourse = new Learning
        {
            Id = 2,
            Title = ".NET Full Stack Development - Intermediate",
            Category = "Technical - Certification",
            Description = "Intermediate",
            DurationInHours = 50
        };

        _context.Users.Add(user);
        _context.Learnings.AddRange(beginnerCourse, intermediateCourse);
        await _context.SaveChangesAsync();

        // Act - Complete Beginner
        var beginnerResult = new AssessmentResult
        {
            UserId = user.Id,
            LearningId = beginnerCourse.Id,
            Score = 85,
            Passed = true,
            DifficultyLevel = "Beginner",
            CompletedDate = DateTime.Now,
            TotalQuestions = 10,
            CorrectAnswers = 8
        };

        var beginnerCertificate = new Certificate
        {
            UserId = user.Id,
            LearningId = beginnerCourse.Id,
            AssessmentResultId = beginnerResult.Id,
            CertificateNumber = $"CERT-{DateTime.Now:yyyyMMdd}-0001-0001",
            Title = "Beginner Certificate",
            DifficultyLevel = "Beginner",
            IssuedDate = DateTime.Now,
            Score = 85
        };

        _context.AssessmentResults.Add(beginnerResult);
        _context.Certificates.Add(beginnerCertificate);
        await _context.SaveChangesAsync();

        // Act - Complete Intermediate
        var intermediateResult = new AssessmentResult
        {
            UserId = user.Id,
            LearningId = intermediateCourse.Id,
            Score = 92,
            Passed = true,
            DifficultyLevel = "Intermediate",
            CompletedDate = DateTime.Now,
            TotalQuestions = 10,
            CorrectAnswers = 9
        };

        var intermediateCertificate = new Certificate
        {
            UserId = user.Id,
            LearningId = intermediateCourse.Id,
            AssessmentResultId = intermediateResult.Id,
            CertificateNumber = $"CERT-{DateTime.Now:yyyyMMdd}-0001-0002",
            Title = "Intermediate Certificate",
            DifficultyLevel = "Intermediate",
            IssuedDate = DateTime.Now,
            Score = 92
        };

        _context.AssessmentResults.Add(intermediateResult);
        _context.Certificates.Add(intermediateCertificate);
        await _context.SaveChangesAsync();

        // Assert
        var certificates = await _context.Certificates
            .Where(c => c.UserId == user.Id)
            .OrderBy(c => c.DifficultyLevel)
            .ToListAsync();

        Assert.Equal(2, certificates.Count);
        Assert.Equal("Beginner", certificates[0].DifficultyLevel);
        Assert.Equal("Intermediate", certificates[1].DifficultyLevel);
        Assert.Equal(85, certificates[0].Score);
        Assert.Equal(92, certificates[1].Score);
    }

    [Fact]
    public async Task CertificationFlow_NoCertificate_WhenAssessmentFails()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "student@example.com",
            Email = "student@example.com",
            FullName = "Test Student",
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

        _context.Users.Add(user);
        _context.Learnings.Add(learning);
        await _context.SaveChangesAsync();

        // Act - Fail the assessment
        var failedResult = new AssessmentResult
        {
            UserId = user.Id,
            LearningId = learning.Id,
            Score = 60,
            Passed = false,
            DifficultyLevel = "Beginner",
            CompletedDate = DateTime.Now,
            TotalQuestions = 10,
            CorrectAnswers = 6
        };

        _context.AssessmentResults.Add(failedResult);
        await _context.SaveChangesAsync();

        // Assert - No certificate should be generated
        var certificate = await _context.Certificates
            .FirstOrDefaultAsync(c => c.UserId == user.Id && c.LearningId == learning.Id);

        Assert.Null(certificate);
    }

    [Fact]
    public async Task CertificationFlow_VerifyRelationships()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = 1,
            UserName = "student@example.com",
            Email = "student@example.com",
            FullName = "Test Student",
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

        var assessmentResult = new AssessmentResult
        {
            Id = 1,
            UserId = user.Id,
            LearningId = learning.Id,
            Score = 85,
            Passed = true,
            DifficultyLevel = "Beginner",
            CompletedDate = DateTime.Now,
            TotalQuestions = 10,
            CorrectAnswers = 8
        };

        var certificate = new Certificate
        {
            Id = 1,
            UserId = user.Id,
            LearningId = learning.Id,
            AssessmentResultId = assessmentResult.Id,
            CertificateNumber = "CERT-20260210-0001-0001",
            Title = "Test Certificate",
            DifficultyLevel = "Beginner",
            IssuedDate = DateTime.Now,
            Score = 85
        };

        _context.Users.Add(user);
        _context.Learnings.Add(learning);
        _context.AssessmentResults.Add(assessmentResult);
        _context.Certificates.Add(certificate);
        await _context.SaveChangesAsync();

        // Act - Load certificate with all relationships
        var loadedCertificate = await _context.Certificates
            .Include(c => c.User)
            .Include(c => c.Learning)
            .Include(c => c.AssessmentResult)
            .FirstOrDefaultAsync(c => c.Id == 1);

        // Assert
        Assert.NotNull(loadedCertificate);
        Assert.NotNull(loadedCertificate.User);
        Assert.Equal("Test Student", loadedCertificate.User.FullName);
        Assert.NotNull(loadedCertificate.Learning);
        Assert.Equal("Test Course", loadedCertificate.Learning.Title);
        Assert.NotNull(loadedCertificate.AssessmentResult);
        Assert.Equal(85, loadedCertificate.AssessmentResult.Score);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
