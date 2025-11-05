using System.ComponentModel.DataAnnotations;

namespace HrCertificatePortal.Api.DTOs
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeEmail { get; set; } = string.Empty;
        public string? Batch { get; set; }
    }

    public class CreateEmployeeDto
    {
        [Required]
        [MaxLength(200)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string EmployeeEmail { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Batch { get; set; }
    }

    public class BulkUploadResultDto
    {
        public int Total { get; set; }
        public int Inserted { get; set; }
        public int Failed { get; set; }
    }
}
