# HR Certificate Portal Backend (ASP.NET Core 8)

## Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB is fine)  
- PowerShell (Windows)

## Configuration
- Edit `HrCertificatePortal.Api/appsettings.json`:
  - `ConnectionStrings:DefaultConnection` to point to your SQL Server
  - `Jwt` section: set a strong `Key`, and adjust `Issuer`/`Audience` if needed

## Restore & Build
```powershell
# From the backend root
dotnet restore .\HrCertificatePortal.Api\HrCertificatePortal.Api.csproj
dotnet build -c Release .\HrCertificatePortal.Api\HrCertificatePortal.Api.csproj
```

## Run Locally
```powershell
# Development run
 dotnet run --project .\HrCertificatePortal.Api\HrCertificatePortal.Api.csproj
```
The API will expose Swagger UI at `/swagger`.

## Database & Migrations
This project calls `EnsureCreated()` at startup for quick local development.
To use EF Core migrations instead:
```powershell
# Install dotnet-ef tool if needed
 dotnet tool install --global dotnet-ef

# Add initial migration
 dotnet ef migrations add InitialCreate --project .\HrCertificatePortal.Api\HrCertificatePortal.Api.csproj --startup-project .\HrCertificatePortal.Api\HrCertificatePortal.Api.csproj

# Apply migration
 dotnet ef database update --project .\HrCertificatePortal.Api\HrCertificatePortal.Api.csproj --startup-project .\HrCertificatePortal.Api\HrCertificatePortal.Api.csproj
```

## Default Seed Users
- Admin: `admin@example.com` / `Admin@123`
- Super Admin: `superadmin@example.com` / `Super@123`

Use `/api/auth/login` to obtain a JWT, then authorize in Swagger with `Bearer <token>`.
