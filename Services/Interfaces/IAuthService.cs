using HrCertificatePortal.Api.DTOs;

namespace HrCertificatePortal.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
        Task<bool> RegisterAsync(RegisterRequestDto dto);
    }
}
