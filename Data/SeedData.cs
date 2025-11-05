using HrCertificatePortal.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HrCertificatePortal.Api.Data
{
    public static class SeedData
    {
        public static async Task SeedInitialDataAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var hasher = services.GetRequiredService<IPasswordHasher<User>>();

            // Database creation is handled in Program via EnsureCreated for dev.

            // Seed Admin
            var adminEmail = "admin@example.com";
            if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var admin = new User
                {
                    Email = adminEmail,
                    Role = "Admin",
                    CreatedBy = "seed"
                };
                admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
                context.Users.Add(admin);
            }

            // Seed Super Admin
            var superEmail = "superadmin@example.com";
            if (!await context.Users.AnyAsync(u => u.Email == superEmail))
            {
                var super = new User
                {
                    Email = superEmail,
                    Role = "Super Admin",
                    CreatedBy = "seed"
                };
                super.PasswordHash = hasher.HashPassword(super, "Super@123");
                context.Users.Add(super);
            }

            // Optionally seed a sample course with null Template
            if (!await context.Courses.AnyAsync())
            {
                context.Courses.Add(new Course
                {
                    CourseName = "Sample Course",
                    CourseCode = "SC-001",
                    Template = null,
                    CreatedBy = "seed"
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
