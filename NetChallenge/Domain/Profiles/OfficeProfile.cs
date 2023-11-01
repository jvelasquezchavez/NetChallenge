using AutoMapper;
using NetChallenge.Dto.Output;

namespace NetChallenge.Domain.Profiles
{
    public class OfficeProfile : Profile
    {
        public OfficeProfile() 
        {
            CreateMap<Office, OfficeDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(res => res.Location.Name));
        }
    }
}
