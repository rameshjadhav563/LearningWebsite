namespace LearningWebsite.Models
{
    public class Question
    {
        public int Id { get; set; }
        public int LearningId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty; // A, B, C, or D
        public string DifficultyLevel { get; set; } = "Beginner"; // Beginner, Intermediate, Advanced

        // Navigation property
        public Learning? Learning { get; set; }
    }
}
