using System.ComponentModel.DataAnnotations;

namespace HrCertificatePortal.Api.Models
{
    public class Course : AuditableEntity
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CourseCode { get; set; } = string.Empty;

        public byte[]? Template { get; set; } // Stores JPEG bytes directly in DB

        [MaxLength(2048)]
        public string? CertificateUrl { get; set; }

        [MaxLength(2048)]
        public string? LinkedHashtagMessage { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
