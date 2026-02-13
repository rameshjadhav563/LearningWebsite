using LearningWebsite.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningWebsite.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; } = null!;
        public DbSet<Learning> Learnings { get; set; } = null!;
        public DbSet<LearningAssignment> LearningAssignments { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<AssessmentResult> AssessmentResults { get; set; } = null!;
        public DbSet<AssessmentAnswerDetail> AssessmentAnswerDetails { get; set; } = null!;
        public DbSet<Certificate> Certificates { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map ApplicationUser to its own Users table so seeding works independently of Identity
            modelBuilder.Entity<ApplicationUser>().ToTable("Users");

            // Manager-Employee relationship
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Manager)
                .WithMany(u => u.TeamMembers)
                .HasForeignKey(u => u.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);

            // Configure relationships
            modelBuilder.Entity<LearningAssignment>()
                .HasOne(la => la.User)
                .WithMany(u => u.Assignments)
                .HasForeignKey(la => la.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LearningAssignment>()
                .HasOne(la => la.Learning)
                .WithMany(l => l.Assignments)
                .HasForeignKey(la => la.LearningId)
                .OnDelete(DeleteBehavior.Cascade);

            // Question-Learning relationship
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Learning)
                .WithMany()
                .HasForeignKey(q => q.LearningId)
                .OnDelete(DeleteBehavior.Cascade);

            // AssessmentResult relationships
            modelBuilder.Entity<AssessmentResult>()
                .HasOne(ar => ar.User)
                .WithMany()
                .HasForeignKey(ar => ar.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AssessmentResult>()
                .HasOne(ar => ar.Learning)
                .WithMany()
                .HasForeignKey(ar => ar.LearningId)
                .OnDelete(DeleteBehavior.Cascade);

            // AssessmentAnswerDetail relationships
            modelBuilder.Entity<AssessmentAnswerDetail>()
                .HasOne(aad => aad.AssessmentResult)
                .WithMany(ar => ar.AnswerDetails)
                .HasForeignKey(aad => aad.AssessmentResultId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssessmentAnswerDetail>()
                .HasOne(aad => aad.Question)
                .WithMany()
                .HasForeignKey(aad => aad.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);  // Prevent cascade delete conflict

            // Configure decimal precision for Score
            modelBuilder.Entity<AssessmentResult>()
                .Property(ar => ar.Score)
                .HasPrecision(5, 2);
        }
    }
}
