using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Models;
using Microsoft.AspNetCore.Http;

namespace HrCertificatePortal.Api.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetByCourseAsync(int courseId);
        Task<Employee> AddAsync(int courseId, CreateEmployeeDto dto, string? createdBy);
        Task<BulkUploadResultDto> BulkUploadAsync(int courseId, IFormFile excelFile, string? createdBy);
        Task<byte[]> GenerateTemplateAsync();
    }
}
