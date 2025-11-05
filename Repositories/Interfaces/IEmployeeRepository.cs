using HrCertificatePortal.Api.Models;

namespace HrCertificatePortal.Api.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetByCourseAsync(int courseId);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> GetByEmailAndCourseAsync(string email, int courseId);
        Task AddAsync(Employee employee);
        Task AddRangeAsync(IEnumerable<Employee> employees);
        Task SaveChangesAsync();
    }
}
