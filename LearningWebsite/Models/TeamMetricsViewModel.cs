namespace LearningWebsite.Models
{
    public class TeamMetricsViewModel
    {
        public int TotalTeamMembers { get; set; }
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        public int InProgressAssignments { get; set; }
        public int NotStartedAssignments { get; set; }
        public int OverdueAssignments { get; set; }
        public double TeamCompletionRate { get; set; }
        public double AverageProgressPercentage { get; set; }

        // Team member individual metrics (paginated)
        public PaginatedList<TeamMemberMetric> TeamMemberMetrics { get; set; } = new(new List<TeamMemberMetric>(), 0, 1, 10);

        // Learning category metrics (paginated)
        public PaginatedList<CategoryMetric> CategoryMetrics { get; set; } = new(new List<CategoryMetric>(), 0, 1, 10);

        // Recent activity (paginated)
        public PaginatedList<RecentActivity> RecentActivities { get; set; } = new(new List<RecentActivity>(), 0, 1, 10);
    }

    public class TeamMemberMetric
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        public int InProgressAssignments { get; set; }
        public int NotStartedAssignments { get; set; }
        public int OverdueAssignments { get; set; }
        public double CompletionRate { get; set; }
        public double AverageProgress { get; set; }
    }

    public class CategoryMetric
    {
        public string Category { get; set; } = string.Empty;
        public int TotalAssignments { get; set; }
        public int CompletedAssignments { get; set; }
        public double CompletionRate { get; set; }
    }

    public class RecentActivity
    {
        public string UserName { get; set; } = string.Empty;
        public string LearningTitle { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string ActivityType { get; set; } = string.Empty; // Assigned, Completed, Started
    }
}
