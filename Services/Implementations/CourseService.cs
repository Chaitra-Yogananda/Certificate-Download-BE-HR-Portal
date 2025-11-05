using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Models;
using HrCertificatePortal.Api.Repositories.Interfaces;
using HrCertificatePortal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HrCertificatePortal.Api.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private const long MaxImageBytes = 5L * 1024 * 1024; // 5 MB
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg" };
        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase) {
            "image/jpeg"
        };

        private readonly ICourseRepository _courseRepo;
        private readonly IConfiguration _config;

        public CourseService(ICourseRepository courseRepo, IConfiguration config)
        {
            _courseRepo = courseRepo;
            _config = config;
        }

        public async Task<List<Course>> GetAllAsync() => await _courseRepo.GetAllAsync();

        public async Task<Course?> GetByIdAsync(int id) => await _courseRepo.GetByIdAsync(id);

        public async Task<Course> CreateAsync(CreateCourseDto dto, IFormFile templateFile, string? createdBy)
        {
            ValidateImage(templateFile);

            byte[]? bytes = await ReadBytesAsync(templateFile);

            var course = new Course
            {
                CourseName = dto.CourseName.Trim(),
                CourseCode = dto.CourseCode.Trim(),
                Template = bytes,
                LinkedHashtagMessage = string.IsNullOrWhiteSpace(dto.LinkedHashtagMessage) ? null : dto.LinkedHashtagMessage!.Trim(),
                CreatedBy = createdBy
            };

            // Build CertificateUrl: {BASE_URL}/{CourseCode}/{randomString}
            var baseUrl = (_config["App:BaseUrl"] ?? _config["BaseUrl"] ?? "http://localhost").TrimEnd('/');
            var safeCode = Uri.EscapeDataString(course.CourseCode);
            var random = Guid.NewGuid().ToString("N").Substring(0, 8); // 8 hex chars
            course.CertificateUrl = $"{baseUrl}/{safeCode}/{random}";

            await _courseRepo.AddAsync(course);
            await _courseRepo.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> UpdateAsync(int id, UpdateCourseDto dto, IFormFile? templateFile, string? modifiedBy)
        {
            var course = await _courseRepo.GetByIdAsync(id);
            if (course == null) return null;

            course.CourseName = dto.CourseName.Trim();
            course.CourseCode = dto.CourseCode.Trim();
            course.LinkedHashtagMessage = string.IsNullOrWhiteSpace(dto.LinkedHashtagMessage) ? null : dto.LinkedHashtagMessage!.Trim();
            course.ModifiedBy = modifiedBy;

            if (templateFile != null)
            {
                ValidateImage(templateFile);
                course.Template = await ReadBytesAsync(templateFile);
            }

            await _courseRepo.UpdateAsync(course);
            await _courseRepo.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteAsync(int id, string? modifiedBy)
        {
            var course = await _courseRepo.GetByIdAsync(id);
            if (course == null) return false;
            course.ModifiedBy = modifiedBy;
            await _courseRepo.DeleteAsync(course);
            await _courseRepo.SaveChangesAsync();
            return true;
        }

        private static void ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Template image is required.");
            if (file.Length > MaxImageBytes)
                throw new ArgumentException("Template image exceeds 5 MB size limit.");

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                throw new ArgumentException("Only .jpg or .jpeg files are allowed.");

            if (!string.IsNullOrEmpty(file.ContentType) && !AllowedContentTypes.Contains(file.ContentType))
                throw new ArgumentException("Invalid content type. Only image/jpeg is allowed.");
        }

        private static async Task<byte[]> ReadBytesAsync(IFormFile file)
        {
            using var ms = new MemoryStream((int)file.Length);
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
