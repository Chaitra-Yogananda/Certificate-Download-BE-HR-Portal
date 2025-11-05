using AutoMapper;
using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrCertificatePortal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllAsync();
            var result = _mapper.Map<List<CourseDto>>(courses);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var course = await _courseService.GetByIdAsync(id);
            if (course == null) return NotFound();
            var dto = _mapper.Map<CourseDto>(course);
            return Ok(dto);
        }

        [HttpPost]
        [RequestSizeLimit(6_000_000)] // 6MB just above 5MB validation
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateCourseDto model, [FromForm] IFormFile Template)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBy = User?.Identity?.Name;
            var course = await _courseService.CreateAsync(model, Template, createdBy);
            var dto = _mapper.Map<CourseDto>(course);
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, dto);
        }

        [HttpPut("{id}")]
        [RequestSizeLimit(6_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateCourseDto model, [FromForm] IFormFile? Template)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var modifiedBy = User?.Identity?.Name;
            var course = await _courseService.UpdateAsync(id, model, Template, modifiedBy);
            if (course == null) return NotFound();
            var dto = _mapper.Map<CourseDto>(course);
            return Ok(dto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var modifiedBy = User?.Identity?.Name;
            var ok = await _courseService.DeleteAsync(id, modifiedBy);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
