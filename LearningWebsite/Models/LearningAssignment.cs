namespace LearningWebsite.Models
{
    public class LearningAssignment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int LearningId { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = "NotStarted"; // NotStarted, InProgress, Completed
        public int? ProgressPercentage { get; set; }
        public DateTime? CompletedDate { get; set; }

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public Learning? Learning { get; set; }
    }
}
