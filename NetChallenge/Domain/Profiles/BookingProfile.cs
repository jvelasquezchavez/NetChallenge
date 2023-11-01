using AutoMapper;
using NetChallenge.Dto.Output;

namespace NetChallenge.Domain.Profiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.OfficeName, opt => opt.MapFrom(res => res.Office.Name))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(res => res.Office.Location.Name));
        }
    }
}
