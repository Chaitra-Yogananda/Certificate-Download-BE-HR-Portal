using System.ComponentModel.DataAnnotations;

namespace HrCertificatePortal.Api.Models
{
    public class User : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Admin"; // Allowed: Admin, Super Admin
    }
}
