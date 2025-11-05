using HrCertificatePortal.Api.Services.Interfaces;
using HrCertificatePortal.Api.Repositories.Interfaces;
using HrCertificatePortal.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace HrCertificatePortal.Api.Controllers
{
    [ApiController]
    [Route("api/certificates")]
    public class CertificateController : ControllerBase
    {
        private readonly ICertificateService _certificateService;
        private readonly ICourseRepository _courseRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public CertificateController(ICertificateService certificateService, ICourseRepository courseRepository, IEmployeeRepository employeeRepository)
        {
            _certificateService = certificateService;
            _courseRepository = courseRepository;
            _employeeRepository = employeeRepository;
        }

        [HttpGet("download/{courseCode}")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadCertificate(string courseCode, [FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(courseCode) || string.IsNullOrWhiteSpace(email))
                return BadRequest(new { error = "Course code and email are required." });

            var pdfBytes = await _certificateService.GenerateCertificateAsync(courseCode, email);
            if (pdfBytes == null)
                return StatusCode(403, new { error = "You are not eligible to download the Certificate" });

            return File(pdfBytes, "application/pdf", $"Certificate_{courseCode}.pdf");
        }

        // Compatibility: singular path
        [HttpGet("~/api/certificate/download/{courseCode}")]
        [AllowAnonymous]
        public Task<IActionResult> DownloadCertificateSingular(string courseCode, [FromQuery] string email)
            => DownloadCertificate(courseCode, email);

        // Compatibility: accept an extra slug segment (e.g., /download/{courseCode}/{random})
        [HttpGet("download/{courseCode}/{slug}")]
        [AllowAnonymous]
        public Task<IActionResult> DownloadCertificateWithSlug(string courseCode, string slug, [FromQuery] string email)
            => DownloadCertificate(courseCode, email);

        [HttpPost("validate-user")]
        [HttpPost("~/api/certificate/validate-user")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateUser([FromBody] ValidateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseRepository.GetByCodeAsync(request.CourseCode);
            if (course == null)
                return Ok(new ValidateUserResponse { Valid = false });

            var emp = await _employeeRepository.GetByEmailAndCourseAsync(request.Email, course.Id);
            return Ok(new ValidateUserResponse { Valid = emp != null });
        }

        [HttpGet("details")]
        [HttpGet("~/api/certificate/details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDetails([FromQuery] string email, [FromQuery] string courseCode, [FromQuery] bool includeTemplate = false)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(courseCode))
                return BadRequest(new { error = "Email and courseCode are required." });

            var course = await _courseRepository.GetByCodeAsync(courseCode);
            if (course == null)
                return NotFound(new { error = "Course not found" });

            var emp = await _employeeRepository.GetByEmailAndCourseAsync(email, course.Id);
            if (emp == null)
                return StatusCode(403, new { error = "You are not eligible to download the Certificate" });

            var dto = new CertificateDetailsResponse
            {
                UserName = emp.EmployeeName,
                CourseName = course.CourseName,
                TemplateExists = course.Template != null && course.Template.Length > 0,
                LinkedHashtagMessage = course.LinkedHashtagMessage
            };

            if (includeTemplate && course.Template != null && course.Template.Length > 0)
            {
                // Return as base64 to avoid content-type complexities over JSON
                var base64 = Convert.ToBase64String(course.Template);
                // Using dynamic to avoid changing DTO signature if unnecessary elsewhere
                return Ok(new
                {
                    dto.UserName,
                    dto.CourseName,
                    dto.TemplateExists,
                    TemplateBase64 = base64,
                    dto.LinkedHashtagMessage
                });
            }

            return Ok(dto);
        }

        [HttpPost("generate")]
        [HttpPost("~/api/certificate/generate")]
        [AllowAnonymous]
        public async Task<IActionResult> Generate([FromBody] GenerateCertificateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = await _courseRepository.GetByCodeAsync(request.CourseCode);
            if (course == null)
                return NotFound(new { error = "Course not found" });

            var emp = await _employeeRepository.GetByEmailAndCourseAsync(request.Email, course.Id);
            if (emp == null)
                return StatusCode(403, new { error = "You are not eligible to download the Certificate" });

            var pdf = await _certificateService.GenerateCertificateAsync(request.CourseCode, request.Email);
            if (pdf == null)
                return StatusCode(500, new { error = "Failed to generate certificate" });

            var safeUser = string.Join("_", (emp.EmployeeName ?? "User").Split(Path.GetInvalidFileNameChars()));
            var safeCourse = string.Join("_", (course.CourseName ?? "Course").Split(Path.GetInvalidFileNameChars()));
            var fileName = $"{safeUser}_{safeCourse}.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}
