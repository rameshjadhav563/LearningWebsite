using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningWebsite.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Role { get; set; } = string.Empty; // Employee, Manager, HR

        [StringLength(256)]
        public string? FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        // Manager-Employee relationship
        public int? ManagerId { get; set; }

        [ForeignKey("ManagerId")]
        public ApplicationUser? Manager { get; set; }

        // Navigation properties
        public ICollection<ApplicationUser> TeamMembers { get; set; } = new List<ApplicationUser>();
        public ICollection<LearningAssignment> Assignments { get; set; } = new List<LearningAssignment>();
    }
}