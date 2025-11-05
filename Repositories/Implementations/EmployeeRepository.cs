using HrCertificatePortal.Api.Data;
using HrCertificatePortal.Api.Models;
using HrCertificatePortal.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HrCertificatePortal.Api.Repositories.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _db;
        public EmployeeRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Employee>> GetByCourseAsync(int courseId)
        {
            return await _db.Employees.AsNoTracking().Where(e => e.CourseId == courseId).ToListAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _db.Employees.FindAsync(id);
        }

        public async Task<Employee?> GetByEmailAndCourseAsync(string email, int courseId)
        {
            return await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.EmployeeEmail == email && e.CourseId == courseId);
        }

        public async Task AddAsync(Employee employee)
        {
            await _db.Employees.AddAsync(employee);
        }

        public async Task AddRangeAsync(IEnumerable<Employee> employees)
        {
            await _db.Employees.AddRangeAsync(employees);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}

