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

        public OfficeRentalService(ILocationRepository locationRepository, IOfficeRepository officeRepository, IBookingRepository bookingRepository)
        {
            _locationRepository = locationRepository;
            _officeRepository = officeRepository;
            _bookingRepository = bookingRepository;
        }

        public void AddLocation(AddLocationRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name))
                    throw new LocationNameNullOrEmptyException();

                if (string.IsNullOrEmpty(request.Neighborhood))
                    throw new LocationNeighborhoodNullOrEmptyException();

                Location newLocation = new Location(request.Name, request.Neighborhood);

                if (GetLocations(newLocation.Name).Any())
                    throw new LocationNameDuplicateException();

                _locationRepository.Add(newLocation);
            }
            catch (LocationNameDuplicateException ex)
            {
                // Aquí puedes manejar la excepción, registrarla o realizar acciones específicas si es necesario.
                // Por ejemplo, puedes registrar el error y devolver un mensaje de error al usuario.
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
                Location location = LocationMapper.MapToLocation(GetLocations(request.LocationName).FirstOrDefault());

                if (location == null)
                {
                    throw new InvalidOperationException("La ubicación especificada no existe.");
                }

                // Validar que no exista una oficina con el mismo nombre en la ubicación
                //if (GetOffices(location.Name).Any())
                //    throw new OfficeHasSameNameAsLocationException();

                if (GetOffice(request.LocationName, request.Name) != null)
                    throw new OfficeNameDuplicateException();

                // Crear una lista de recursos disponibles (puede ser vacía si no se proporciona)
                IEnumerable<string> availableResources = request.AvailableResources ?? new List<string>();

                // Crear la nueva oficina
                Office newOffice = new Office(location, request.Name, request.MaxCapacity, availableResources);

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
            catch (OfficeHasSameNameAsLocationException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                throw;
            }
        }

        public void BookOffice(BookOfficeRequest request)
        {
            if (!GetLocations(request.LocationName).Any())
                throw new LocationNotFoundException(request.LocationName);

            if (GetOffice(request.LocationName, request.OfficeName) == null)
                throw new OfficeNotFoundException(request.OfficeName);

            if (string.IsNullOrEmpty(request.UserName))
                throw new UserRequiredException();

            if (request.Duration == null || request.Duration <= TimeSpan.Zero)
                throw new InvalidDurationException();

            IEnumerable<BookingDto> bookingDtos = GetBookings(request.LocationName, request.OfficeName);

            if (!CanBookOffice(request, bookingDtos))
                throw new BookingConflictException();

            if (request.Duration.TotalMinutes % 60 != 0)
                throw new InvalidDurationInHoursException();

            Booking booking = new Booking(request.LocationName, request.OfficeName, request.DateTime, request.Duration, request.UserName);
            _bookingRepository.Add(booking);
        }

        public bool CanBookOffice(BookOfficeRequest request, IEnumerable<BookingDto> bookingDtos)
        {
            // Verificar si hay conflictos de horario
            bool isAvailable = !bookingDtos.Any(booking =>
                booking.LocationName == request.LocationName &&
                booking.OfficeName == request.OfficeName &&
                IsTimeSlotOverlap(booking, request)
            );

            return isAvailable;
        }

        private bool IsTimeSlotOverlap(BookingDto existingBooking, BookOfficeRequest newBooking)
        {
            DateTime newBookingStart = newBooking.DateTime;
            DateTime newBookingEnd = newBookingStart.Add(newBooking.Duration);

            DateTime existingBookingStart = existingBooking.DateTime;
            DateTime existingBookingEnd = existingBookingStart.Add(existingBooking.Duration);

            // Verificar si hay solapamiento de horarios
            return newBookingStart < existingBookingEnd && newBookingEnd > existingBookingStart;
        }

        public IEnumerable<BookingDto> GetBookings(string locationName, string officeName)
        {
            List<BookingDto> bookingDtos = new List<BookingDto>();

            foreach (var booking in _bookingRepository.AsEnumerable())
                bookingDtos.Add(BookingMapper.MapToLocationDto(booking));

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
            throw new NotImplementedException();
        }

        public bool LocationAndOfficeWithSameName(string locationName)
        {
            return GetOffices(locationName).Any();
        }
    }
}