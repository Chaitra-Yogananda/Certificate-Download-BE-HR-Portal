using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrCertificatePortal.Api.Models
{
    public class Employee : AuditableEntity
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(200)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string EmployeeEmail { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Batch { get; set; }

        public Course? Course { get; set; }
    }
}
