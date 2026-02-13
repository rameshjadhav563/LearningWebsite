using LearningWebsite.Models;

namespace LearningWebsite.Tests.Models;

public class CertificateTests
{
    [Fact]
    public void Certificate_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var certificate = new Certificate();

        // Assert
        Assert.Equal(0, certificate.Id);
        Assert.Equal(0, certificate.UserId);
        Assert.Equal(0, certificate.LearningId);
        Assert.Equal(0, certificate.AssessmentResultId);
        Assert.Equal(string.Empty, certificate.CertificateNumber);
        Assert.Equal(string.Empty, certificate.Title);
        Assert.Equal(string.Empty, certificate.DifficultyLevel);
        Assert.Equal(default, certificate.IssuedDate);
        Assert.Equal(0, certificate.Score);
    }

    [Fact]
    public void Certificate_ShouldSetAllProperties()
    {
        // Arrange
        var issuedDate = DateTime.Now;
        var certificate = new Certificate
        {
            Id = 1,
            UserId = 100,
            LearningId = 5,
            AssessmentResultId = 10,
            CertificateNumber = "CERT-20260210-0100-0005",
            Title = ".NET Full Stack Development - Beginner Level",
            DifficultyLevel = "Beginner",
            IssuedDate = issuedDate,
            Score = 85.50m
        };

        // Assert
        Assert.Equal(1, certificate.Id);
        Assert.Equal(100, certificate.UserId);
        Assert.Equal(5, certificate.LearningId);
        Assert.Equal(10, certificate.AssessmentResultId);
        Assert.Equal("CERT-20260210-0100-0005", certificate.CertificateNumber);
        Assert.Equal(".NET Full Stack Development - Beginner Level", certificate.Title);
        Assert.Equal("Beginner", certificate.DifficultyLevel);
        Assert.Equal(issuedDate, certificate.IssuedDate);
        Assert.Equal(85.50m, certificate.Score);
    }

    [Theory]
    [InlineData("Beginner")]
    [InlineData("Intermediate")]
    [InlineData("Advanced")]
    public void Certificate_ShouldAcceptValidDifficultyLevels(string difficultyLevel)
    {
        // Arrange & Act
        var certificate = new Certificate
        {
            DifficultyLevel = difficultyLevel
        };

        // Assert
        Assert.Equal(difficultyLevel, certificate.DifficultyLevel);
    }

    [Theory]
    [InlineData(70.0)]
    [InlineData(85.5)]
    [InlineData(100.0)]
    public void Certificate_ShouldAcceptPassingScores(decimal score)
    {
        // Arrange & Act
        var certificate = new Certificate
        {
            Score = score
        };

        // Assert
        Assert.True(certificate.Score >= 70);
        Assert.Equal(score, certificate.Score);
    }

    [Fact]
    public void Certificate_CertificateNumber_ShouldFollowCorrectFormat()
    {
        // Arrange
        var certificate = new Certificate
        {
            CertificateNumber = "CERT-20260210-0100-0005"
        };

        // Assert
        Assert.Matches(@"^CERT-\d{8}-\d{4}-\d{4}$", certificate.CertificateNumber);
    }

    [Fact]
    public void Certificate_NavigationProperties_ShouldBeNullableAndAccessible()
    {
        // Arrange
        var certificate = new Certificate
        {
            User = new ApplicationUser { Id = 1, FullName = "John Doe" },
            Learning = new Learning { Id = 1, Title = "Test Course" },
            AssessmentResult = new AssessmentResult { Id = 1, Score = 90 }
        };

        // Assert
        Assert.NotNull(certificate.User);
        Assert.NotNull(certificate.Learning);
        Assert.NotNull(certificate.AssessmentResult);
        Assert.Equal("John Doe", certificate.User.FullName);
        Assert.Equal("Test Course", certificate.Learning.Title);
        Assert.Equal(90, certificate.AssessmentResult.Score);
    }

    [Fact]
    public void Certificate_IssuedDate_ShouldNotBeInFuture()
    {
        // Arrange
        var certificate = new Certificate
        {
            IssuedDate = DateTime.Now
        };

        // Assert
        Assert.True(certificate.IssuedDate <= DateTime.Now);
    }
}
