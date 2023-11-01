using AutoMapper;
using NetChallenge.Abstractions;
using NetChallenge.Domain.Profiles;
using NetChallenge.Infrastructure;

namespace NetChallenge.Test
{
    public class OfficeRentalServiceTest
    {
        protected OfficeRentalService Service;
        protected ILocationRepository LocationRepository;
        protected IOfficeRepository OfficeRepository;
        protected IBookingRepository BookingRepository;
        protected IMapper Mapper;

        public OfficeRentalServiceTest()
        {
            LocationRepository = new LocationRepository();
            OfficeRepository = new OfficeRepository();
            BookingRepository = new BookingRepository();
            Mapper = CreateMapper();
            Service = new OfficeRentalService(LocationRepository, OfficeRepository, BookingRepository, Mapper);
        }
        
        private static IMapper CreateMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BookingProfile());
                cfg.AddProfile(new LocationProfile());
                cfg.AddProfile(new OfficeProfile());
            });

            return mapperConfig.CreateMapper();
        }
    }
}