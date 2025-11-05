using AutoMapper;
using HrCertificatePortal.Api.DTOs;
using HrCertificatePortal.Api.Models;

namespace HrCertificatePortal.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.TemplateBase64,
                    opt => opt.MapFrom(src => src.Template != null ? Convert.ToBase64String(src.Template) : null));

            CreateMap<Employee, EmployeeDto>();
        }
    }
}
