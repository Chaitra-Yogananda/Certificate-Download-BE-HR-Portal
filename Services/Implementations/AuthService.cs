using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Models;
using HrCertificatePortal.Api.Repositories.Interfaces;
using HrCertificatePortal.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HrCertificatePortal.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, IPasswordHasher<User> passwordHasher, IConfiguration config)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetByEmailAsync(dto.Email);
            if (user == null)
                return null;

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verification == PasswordVerificationResult.Failed)
                return null;

            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(int.TryParse(jwtSection["ExpireMinutes"], out var m) ? m : 60);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new LoginResponseDto { Token = tokenString, ExpiresAtUtc = expires, Role = user.Role, Email = user.Email };
        }

        public async Task<bool> RegisterAsync(RegisterRequestDto dto)
        {
            // allowed roles: Admin, Super Admin
            if (!string.Equals(dto.Role, "Admin", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(dto.Role, "Super Admin", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var existing = await _userRepo.GetByEmailAsync(dto.Email);
            if (existing != null)
                return false;

            var user = new User
            {
                Email = dto.Email,
                Role = string.Equals(dto.Role, "Super Admin", StringComparison.OrdinalIgnoreCase) ? "Super Admin" : "Admin",
                CreatedBy = dto.Email
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();
            return true;
        }
    }
}
