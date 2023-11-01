using AutoMapper;
using NetChallenge.Abstractions;
using NetChallenge.Domain;
using NetChallenge.Dto.Input;
using NetChallenge.Dto.Output;
using NetChallenge.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using static NetChallenge.Exceptions.OfficeRentalException;

namespace NetChallenge
{
    public class OfficeRentalService : IOfficeRentalService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IOfficeRepository _officeRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IMapper _mapper;

        public OfficeRentalService(ILocationRepository locationRepository, IOfficeRepository officeRepository, IBookingRepository bookingRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _officeRepository = officeRepository;
            _bookingRepository = bookingRepository;
            _mapper = mapper;
            //106 a 115 para traer automapper --> Llamar profiles a la carpeta de automappers.
        }

        public void AddLocation(AddLocationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                    throw new LocationNameNullOrEmptyException();

                if (string.IsNullOrEmpty(request.Neighborhood))
                    throw new LocationNeighborhoodNullOrEmptyException();

                if (GetLocations(request.Name).Any())
                    throw new LocationNameDuplicateException(request.Name);

                Location location = new Location();
                location = _mapper.Map<Location>(request);
                _locationRepository.Add(location);
            }
            catch (LocationNameDuplicateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            catch (LocationNameNullOrEmptyException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            catch (LocationNeighborhoodNullOrEmptyException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }

        public void AddOffice(AddOfficeRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                    throw new OfficeNameNullOrEmptyException();

                if (string.IsNullOrEmpty(request.LocationName))
                    throw new LocationNameNullOrEmptyException();

                Location location = _mapper.Map<Location>(GetLocations(request.LocationName).FirstOrDefault());

                if (location == null)
                    throw new LocationNotFoundException(request.LocationName);

                if (request.MaxCapacity <= 0)
                    throw new InvalidCapacityException();

                if (GetOffice(request.LocationName, request.Name) != null)
                    throw new OfficeNameDuplicateException();

                // Crear la nueva oficina
                Office newOffice = new Office();
                newOffice.Location = location;
                newOffice.Name = request.Name;
                newOffice.MaxCapacity = request.MaxCapacity;
                newOffice.AvailableResources = request.AvailableResources ?? new List<string>();

                // Agregar la oficina al repositorio
                _officeRepository.Add(newOffice);
            }
            catch (OfficeNameNullOrEmptyException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            catch (OfficeNameDuplicateException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw ex;
            }
            catch (LocationNameNullOrEmptyException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            catch (InvalidCapacityException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
            catch (LocationNotFoundException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }

        public void BookOffice(BookOfficeRequest request)
        {
            if (string.IsNullOrEmpty(request.OfficeName))
                throw new OfficeNameNullOrEmptyException();

            if (string.IsNullOrEmpty(request.LocationName))
                throw new LocationNameNullOrEmptyException();

            if (GetOffice(request.LocationName, request.OfficeName) == null)
                throw new OfficeNotFoundException(request.OfficeName);

            if (string.IsNullOrEmpty(request.UserName))
                throw new UserRequiredException();

            if (request.Duration == null || request.Duration <= TimeSpan.Zero || request.DateTime == default(DateTime))
                throw new InvalidDurationException();

            if (request.Duration.TotalMinutes % 60 != 0)
                throw new InvalidDurationInHoursException();

            Location location = _mapper.Map<Location>(GetLocations(request.LocationName).SingleOrDefault());

            if (location is null)
                throw new LocationNotFoundException(request.LocationName);

            IEnumerable<BookingDto> bookingDtos = GetBookings(request.LocationName, request.OfficeName);

            if (!CanBookOffice(request, bookingDtos))
                throw new BookingConflictException();

            Booking booking = _mapper.Map<Booking>(request);//location, request.OfficeName, request.DateTime, request.Duration, request.UserName
            _bookingRepository.Add(booking);
        }

        public bool CanBookOffice(BookOfficeRequest request, IEnumerable<BookingDto> bookingDtos)
        {
            return !bookingDtos.Any(booking =>
                booking.LocationName == request.LocationName &&
                booking.OfficeName == request.OfficeName &&
                IsTimeSlotOverlap(booking, request)
            );
        }

        private bool IsTimeSlotOverlap(BookingDto existingBooking, BookOfficeRequest newBooking)
        {
            DateTime newBookingStart = newBooking.DateTime;
            DateTime newBookingEnd = newBookingStart.Add(newBooking.Duration);

            DateTime existingBookingStart = existingBooking.DateTime;
            DateTime existingBookingEnd = existingBookingStart.Add(existingBooking.Duration);

            return newBookingStart < existingBookingEnd && newBookingEnd > existingBookingStart;
        }

        public IEnumerable<BookingDto> GetBookings(string locationName, string officeName)
        {
            List<BookingDto> bookingDtos = new List<BookingDto>();

            foreach (var booking in _bookingRepository.AsEnumerable())
                bookingDtos.Add(BookingMapper.MapToBookingDto(booking));

            return bookingDtos;
        }

        public IEnumerable<LocationDto> GetLocations()
        {
            List<LocationDto> locationDtos = new List<LocationDto>();

            foreach (var location in _locationRepository.AsEnumerable())
                locationDtos.Add(LocationMapper.MapToLocationDto(location));

            return locationDtos;
        }

        public IEnumerable<LocationDto> GetLocations(string locationName)
        {
            List<LocationDto> locationDtos = new List<LocationDto>();
            IEnumerable<Location> locations = _locationRepository.AsEnumerable().Where(x => x.Name == locationName).ToList();
            foreach (var location in locations)
                locationDtos.Add(LocationMapper.MapToLocationDto(location));

            return locationDtos;
        }

        public Office GetOffice(string locationName, string officeName) =>
            _officeRepository.AsEnumerable().FirstOrDefault(x => x.Name == officeName && x.Location.Name == locationName);

        public IEnumerable<OfficeDto> GetOffices(string locationName)
        {
            List<OfficeDto> officeDtos = new List<OfficeDto>();
            IEnumerable<Office> offices = _officeRepository.AsEnumerable().Where(x => x.Location.Name == locationName).ToList();

            foreach (var office in offices)
                officeDtos.Add(OfficeMapper.MapToOfficeDto(office));

            return officeDtos;
        }

        public IEnumerable<OfficeDto> GetOfficeSuggestions(SuggestionsRequest request)
        {
            IEnumerable<Office> offices = _officeRepository.AsEnumerable()
                .Where(x => x.MaxCapacity >= request.CapacityNeeded
                        && request.ResourcesNeeded.All(resource => x.AvailableResources.Contains(resource)))
                .OrderBy(x => x.Location.Neighborhood == request.PreferedNeigborHood ? 0 : 1)
                .ThenBy(x => x.MaxCapacity)
                .ThenBy(x => x.AvailableResources.Count());

            List<OfficeDto> officeDtos = new List<OfficeDto>();

            foreach (Office office in offices)
                officeDtos.Add(OfficeMapper.MapToOfficeDto(office));

            return officeDtos;
        }
    }
}