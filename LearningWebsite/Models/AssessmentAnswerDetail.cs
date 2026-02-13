namespace LearningWebsite.Models
{
    public class AssessmentAnswerDetail
    {
        public int Id { get; set; }
        public int AssessmentResultId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string UserAnswer { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

        // Navigation properties
        public AssessmentResult? AssessmentResult { get; set; }
        public Question? Question { get; set; }
    }
}
