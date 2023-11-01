using AutoMapper;
using NetChallenge.Abstractions;
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
            Service = new OfficeRentalService(LocationRepository, OfficeRepository, BookingRepository, Mapper);
        }
    }
}