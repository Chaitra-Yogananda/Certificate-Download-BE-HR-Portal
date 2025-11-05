using HrCertificatePortal.Api.Data;
using HrCertificatePortal.Api.Models;
using HrCertificatePortal.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HrCertificatePortal.Api.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _db;
        public CourseRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            return await _db.Courses.AsNoTracking().ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _db.Courses.Include(c => c.Employees).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Course?> GetByCodeAsync(string courseCode)
        {
            return await _db.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseCode == courseCode);
        }

        public async Task AddAsync(Course course)
        {
            await _db.Courses.AddAsync(course);
        }

        public async Task UpdateAsync(Course course)
        {
            _db.Courses.Update(course);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Course course)
        {
            course.IsDeleted = true; // soft delete as per architecture
            _db.Courses.Update(course);
            await Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}

