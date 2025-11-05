using System.Threading.Tasks;

namespace HrCertificatePortal.Api.Services.Interfaces
{
    public interface ICertificateService
    {
        Task<byte[]?> GenerateCertificateAsync(string courseCode, string email);
    }
}
