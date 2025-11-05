using System.Globalization;
using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Models;
using HrCertificatePortal.Api.Repositories.Interfaces;
using HrCertificatePortal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace HrCertificatePortal.Api.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ICourseRepository _courseRepo;

        public EmployeeService(IEmployeeRepository employeeRepo, ICourseRepository courseRepo)
        {
            _employeeRepo = employeeRepo;
            _courseRepo = courseRepo;
        }

        public async Task<List<Employee>> GetByCourseAsync(int courseId)
        {
            return await _employeeRepo.GetByCourseAsync(courseId);
        }

        public async Task<Employee> AddAsync(int courseId, CreateEmployeeDto dto, string? createdBy)
        {
            // minimal validation: ensure course exists
            var course = await _courseRepo.GetByIdAsync(courseId);
            if (course == null) throw new ArgumentException("Invalid CourseId");

            var employee = new Employee
            {
                CourseId = courseId,
                EmployeeName = dto.EmployeeName.Trim(),
                EmployeeEmail = dto.EmployeeEmail.Trim(),
                Batch = string.IsNullOrWhiteSpace(dto.Batch) ? null : dto.Batch.Trim(),
                CreatedBy = createdBy
            };
            await _employeeRepo.AddAsync(employee);
            await _employeeRepo.SaveChangesAsync();
            return employee;
        }

        public async Task<BulkUploadResultDto> BulkUploadAsync(int courseId, IFormFile excelFile, string? createdBy)
        {
            if (excelFile == null || excelFile.Length == 0)
                throw new ArgumentException("Excel file is required.");

            // Ensure course exists
            var course = await _courseRepo.GetByIdAsync(courseId);
            if (course == null) throw new ArgumentException("Invalid CourseId");

            // Accept only .xlsx
            var ext = Path.GetExtension(excelFile.FileName);
            if (!".xlsx".Equals(ext, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only .xlsx files are allowed.");

            var toInsert = new List<Employee>();
            int total = 0, inserted = 0, failed = 0;

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;
                using var package = new ExcelPackage(stream);
                var sheet = package.Workbook.Worksheets.FirstOrDefault();
                if (sheet == null)
                    throw new ArgumentException("Excel sheet not found.");

                // Read headers
                var headerRow = 1;
                var colCount = sheet.Dimension?.Columns ?? 0;
                var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int c = 1; c <= colCount; c++)
                {
                    var header = sheet.Cells[headerRow, c].Text?.Trim();
                    if (!string.IsNullOrEmpty(header))
                        map[header] = c;
                }

                if (!map.ContainsKey("EmployeeName") || !map.ContainsKey("EmployeeEmail"))
                    throw new ArgumentException("Missing required columns: EmployeeName, EmployeeEmail");

                var rowCount = sheet.Dimension?.Rows ?? 0;
                for (int r = headerRow + 1; r <= rowCount; r++)
                {
                    total++;
                    var name = sheet.Cells[r, map["EmployeeName"]].Text?.Trim();
                    var email = sheet.Cells[r, map["EmployeeEmail"]].Text?.Trim();
                    string? batch = null;
                    if (map.TryGetValue("Batch", out var bcol))
                        batch = string.IsNullOrWhiteSpace(sheet.Cells[r, bcol].Text) ? null : sheet.Cells[r, bcol].Text.Trim();

                    if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                    {
                        failed++;
                        continue;
                    }

                    var emp = new Employee
                    {
                        CourseId = courseId,
                        EmployeeName = name,
                        EmployeeEmail = email,
                        Batch = batch,
                        CreatedBy = createdBy
                    };
                    toInsert.Add(emp);
                    inserted++;
                }
            }

            if (toInsert.Count > 0)
            {
                await _employeeRepo.AddRangeAsync(toInsert);
                await _employeeRepo.SaveChangesAsync();
            }

            return new BulkUploadResultDto
            {
                Total = total,
                Inserted = inserted,
                Failed = failed
            };
        }

        public async Task<byte[]> GenerateTemplateAsync()
        {
            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Employees");
            sheet.Cells[1, 1].Value = "EmployeeName";
            sheet.Cells[1, 2].Value = "EmployeeEmail";
            sheet.Cells[1, 3].Value = "Batch"; // optional

            sheet.Cells.AutoFitColumns();
            var bytes = package.GetAsByteArray();
            return await Task.FromResult(bytes);
        }
    }
}
