using System.ComponentModel.DataAnnotations;

namespace HrCertificatePortal.Api.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string? TemplateBase64 { get; set; }
        public string? CertificateUrl { get; set; }
        public string? LinkedHashtagMessage { get; set; }
    }

    public class CreateCourseDto
    {
        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CourseCode { get; set; } = string.Empty;
        // Note: image file is accepted as IFormFile in the controller action
        [MaxLength(2048)]
        public string? LinkedHashtagMessage { get; set; }
    }

    public class UpdateCourseDto
    {
        [Required]
        [MaxLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string CourseCode { get; set; } = string.Empty;
        // Note: image file is accepted as IFormFile in the controller action
        [MaxLength(2048)]
        public string? LinkedHashtagMessage { get; set; }
    }
}
