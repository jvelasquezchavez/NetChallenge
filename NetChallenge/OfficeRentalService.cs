using System;
using System.Collections.Generic;
using System.Linq;
using NetChallenge.Abstractions;
using NetChallenge.Domain;
using NetChallenge.Dto.Input;
using NetChallenge.Dto.Output;
using NetChallenge.Mappers;
using static NetChallenge.Exceptions.OfficeRentalException;

namespace NetChallenge
{
    public class OfficeRentalService: IOfficeRentalService
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
            throw new NotImplementedException();
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
            
            foreach(var location in _locationRepository.AsEnumerable())
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