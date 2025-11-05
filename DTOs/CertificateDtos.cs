using System.ComponentModel.DataAnnotations;

namespace HrCertificatePortal.Api.DTOs
{
    public class ValidateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string CourseCode { get; set; } = string.Empty;
    }

    public class ValidateUserResponse
    {
        public bool Valid { get; set; }
    }

    public class CertificateDetailsResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public bool TemplateExists { get; set; }
        public string? TemplateUrl { get; set; }
        public string? LinkedHashtagMessage { get; set; }
    }

    public class GenerateCertificateRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string CourseCode { get; set; } = string.Empty;
    }
}
