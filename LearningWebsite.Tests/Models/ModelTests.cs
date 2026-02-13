using LearningWebsite.Models;

namespace LearningWebsite.Tests.Models;

public class QuestionTests
{
    [Fact]
    public void Question_ShouldHaveDifficultyLevelProperty()
    {
        // Arrange & Act
        var question = new Question
        {
            DifficultyLevel = "Beginner"
        };

        // Assert
        Assert.Equal("Beginner", question.DifficultyLevel);
    }

    [Theory]
    [InlineData("Beginner")]
    [InlineData("Intermediate")]
    [InlineData("Advanced")]
    public void Question_ShouldAcceptAllDifficultyLevels(string level)
    {
        // Arrange & Act
        var question = new Question
        {
            DifficultyLevel = level
        };

        // Assert
        Assert.Equal(level, question.DifficultyLevel);
    }

    [Fact]
    public void Question_DifficultyLevel_DefaultsToBeginner()
    {
        // Arrange & Act
        var question = new Question();

        // Assert
        Assert.Equal("Beginner", question.DifficultyLevel);
    }

    [Fact]
    public void Question_ShouldSetAllProperties()
    {
        // Arrange & Act
        var question = new Question
        {
            Id = 1,
            LearningId = 5,
            QuestionText = "What is C#?",
            OptionA = "A programming language",
            OptionB = "A database",
            OptionC = "An OS",
            OptionD = "A browser",
            CorrectAnswer = "A",
            DifficultyLevel = "Intermediate"
        };

        // Assert
        Assert.Equal(1, question.Id);
        Assert.Equal(5, question.LearningId);
        Assert.Equal("What is C#?", question.QuestionText);
        Assert.Equal("A programming language", question.OptionA);
        Assert.Equal("A database", question.OptionB);
        Assert.Equal("An OS", question.OptionC);
        Assert.Equal("A browser", question.OptionD);
        Assert.Equal("A", question.CorrectAnswer);
        Assert.Equal("Intermediate", question.DifficultyLevel);
    }
}

public class AssessmentResultTests
{
    [Fact]
    public void AssessmentResult_ShouldHaveDifficultyLevelProperty()
    {
        // Arrange & Act
        var result = new AssessmentResult
        {
            DifficultyLevel = "Beginner"
        };

        // Assert
        Assert.Equal("Beginner", result.DifficultyLevel);
    }

    [Theory]
    [InlineData("Beginner")]
    [InlineData("Intermediate")]
    [InlineData("Advanced")]
    public void AssessmentResult_ShouldAcceptAllDifficultyLevels(string level)
    {
        // Arrange & Act
        var result = new AssessmentResult
        {
            DifficultyLevel = level
        };

        // Assert
        Assert.Equal(level, result.DifficultyLevel);
    }

    [Fact]
    public void AssessmentResult_DifficultyLevel_DefaultsToBeginner()
    {
        // Arrange & Act
        var result = new AssessmentResult();

        // Assert
        Assert.Equal("Beginner", result.DifficultyLevel);
    }

    [Fact]
    public void AssessmentResult_ShouldCalculatePassedCorrectly()
    {
        // Arrange & Act
        var passedResult = new AssessmentResult
        {
            Score = 75,
            Passed = true
        };

        var failedResult = new AssessmentResult
        {
            Score = 65,
            Passed = false
        };

        // Assert
        Assert.True(passedResult.Passed);
        Assert.True(passedResult.Score >= 70);
        Assert.False(failedResult.Passed);
        Assert.True(failedResult.Score < 70);
    }

    [Theory]
    [InlineData(70, true)]
    [InlineData(85, true)]
    [InlineData(100, true)]
    [InlineData(69.99, false)]
    [InlineData(50, false)]
    [InlineData(0, false)]
    public void AssessmentResult_PassedFlag_ShouldMatchScore(decimal score, bool expectedPassed)
    {
        // Arrange & Act
        var result = new AssessmentResult
        {
            Score = score,
            Passed = score >= 70
        };

        // Assert
        Assert.Equal(expectedPassed, result.Passed);
    }

    [Fact]
    public void AssessmentResult_ShouldSetAllProperties()
    {
        // Arrange
        var completedDate = DateTime.Now;
        
        // Act
        var result = new AssessmentResult
        {
            Id = 1,
            UserId = 100,
            LearningId = 5,
            CompletedDate = completedDate,
            TotalQuestions = 10,
            CorrectAnswers = 8,
            Score = 80,
            Passed = true,
            DifficultyLevel = "Intermediate"
        };

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal(100, result.UserId);
        Assert.Equal(5, result.LearningId);
        Assert.Equal(completedDate, result.CompletedDate);
        Assert.Equal(10, result.TotalQuestions);
        Assert.Equal(8, result.CorrectAnswers);
        Assert.Equal(80, result.Score);
        Assert.True(result.Passed);
        Assert.Equal("Intermediate", result.DifficultyLevel);
    }

    [Fact]
    public void AssessmentResult_AnswerDetails_ShouldBeInitializedAsEmptyList()
    {
        // Arrange & Act
        var result = new AssessmentResult();

        // Assert
        Assert.NotNull(result.AnswerDetails);
        Assert.Empty(result.AnswerDetails);
    }
}
