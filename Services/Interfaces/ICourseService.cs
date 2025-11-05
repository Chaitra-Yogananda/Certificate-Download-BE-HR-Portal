using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Models;
using Microsoft.AspNetCore.Http;

namespace HrCertificatePortal.Api.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<Course> CreateAsync(CreateCourseDto dto, IFormFile templateFile, string? createdBy);
        Task<Course?> UpdateAsync(int id, UpdateCourseDto dto, IFormFile? templateFile, string? modifiedBy);
        Task<bool> DeleteAsync(int id, string? modifiedBy);
    }
}
