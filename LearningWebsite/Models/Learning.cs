namespace LearningWebsite.Models
{
    public class Learning
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int DurationInHours { get; set; }

        // Navigation property
        public ICollection<LearningAssignment> Assignments { get; set; } = new List<LearningAssignment>();
    }
}
