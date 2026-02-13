using System.ComponentModel.DataAnnotations;

namespace LearningWebsite.Models
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(256, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(256, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}