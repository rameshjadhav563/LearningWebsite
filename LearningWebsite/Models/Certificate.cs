using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningWebsite.Models
{
    public class Certificate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int LearningId { get; set; }

        [Required]
        public int AssessmentResultId { get; set; }

        [Required]
        [StringLength(100)]
        public string CertificateNumber { get; set; } = string.Empty;

        [Required]
        public DateTime IssuedDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal Score { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DifficultyLevel { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string LearningTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EmployeeName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [ForeignKey("LearningId")]
        public virtual Learning? Learning { get; set; }

        [ForeignKey("AssessmentResultId")]
        public virtual AssessmentResult? AssessmentResult { get; set; }
    }
}
