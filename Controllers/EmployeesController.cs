using AutoMapper;
using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Models;
using HrCertificatePortal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrCertificatePortal.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin, Super Admin")]
    [Route("api")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeesController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET /api/courses/{id}/employees
        [HttpGet("courses/{id}/employees")]
        public async Task<IActionResult> GetByCourse([FromRoute] int id)
        {
            var employees = await _employeeService.GetByCourseAsync(id);
            var dtos = _mapper.Map<List<EmployeeDto>>(employees);
            return Ok(dtos);
        }

        // POST /api/courses/{id}/employees
        [HttpPost("courses/{id}/employees")]
        public async Task<IActionResult> Add([FromRoute] int id, [FromBody] CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBy = User?.Identity?.Name;
            var employee = await _employeeService.AddAsync(id, dto, createdBy);
            var result = _mapper.Map<EmployeeDto>(employee);
            return Created($"/api/courses/{id}/employees/{result.Id}", result);
        }

        // POST /api/courses/{id}/employees/upload
        [HttpPost("courses/{id}/employees/upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromRoute] int id, [FromForm(Name = "file")] IFormFile file)
        {
            var createdBy = User?.Identity?.Name;
            var summary = await _employeeService.BulkUploadAsync(id, file, createdBy);
            return Ok(summary);
        }

        // GET /api/employees/download-template
        [HttpGet("employees/download-template")]
        public async Task<IActionResult> DownloadTemplate()
        {
            var content = await _employeeService.GenerateTemplateAsync();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "EmployeeUploadTemplate.xlsx";
            return File(content, contentType, fileName);
        }
    }
}
