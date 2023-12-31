﻿using AutoMapper;
using NetChallenge.Abstractions;
using NetChallenge.Domain;
using NetChallenge.Dto.Input;
using NetChallenge.Dto.Output;
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

                if (request.MaxCapacity <= 0)
                    throw new InvalidCapacityException();

                if (GetOffice(request.LocationName, request.Name) != null)
                    throw new OfficeNameDuplicateException();

                Location location = _mapper.Map<Location>(GetLocations(request.LocationName).FirstOrDefault());

                if (location == null)
                    throw new LocationNotFoundException(request.LocationName);

                Office newOffice = new Office();
                newOffice = _mapper.Map<Office>(request);
                newOffice.Location = location;

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

            if (string.IsNullOrEmpty(request.UserName))
                throw new UserRequiredException();

            if (request.Duration == null || request.Duration <= TimeSpan.Zero || request.DateTime == default(DateTime))
                throw new InvalidDurationException();

            if (request.Duration.TotalMinutes % 60 != 0)
                throw new InvalidDurationInHoursException();

            if (GetOffice(request.LocationName, request.OfficeName) == null)
                throw new OfficeNotFoundException(request.OfficeName);

            Location location = _mapper.Map<Location>(GetLocations(request.LocationName).SingleOrDefault());

            if (location is null)
                throw new LocationNotFoundException(request.LocationName);

            Office office = _mapper.Map<Office>(GetOffice(request.LocationName, request.OfficeName));

            if (office is null)
                throw new OfficeNotFoundException(request.LocationName);

            IEnumerable<BookingDto> bookingDtos = GetBookings(request.LocationName, request.OfficeName);

            if (!CanBookOffice(request, bookingDtos))
                throw new BookingConflictException();

            Booking booking = _mapper.Map<Booking>(request);
            booking.Office = office;
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
                bookingDtos.Add(_mapper.Map<BookingDto>(booking));

            return bookingDtos;
        }

        public IEnumerable<LocationDto> GetLocations() => MapList<Location, LocationDto>(_locationRepository.AsEnumerable());

        public IEnumerable<LocationDto> GetLocations(string locationName)
        {
            IEnumerable<Location> locations = _locationRepository.AsEnumerable().Where(x => x.Name == locationName).ToList();

            return MapList<Location, LocationDto>(locations);
        }

        public Office GetOffice(string locationName, string officeName) =>
            _officeRepository.AsEnumerable().FirstOrDefault(x => x.Name == officeName && x.Location.Name == locationName);

        public IEnumerable<OfficeDto> GetOffices(string locationName)
        {
            IEnumerable<Office> offices = _officeRepository.AsEnumerable().Where(x => x.Location.Name == locationName).ToList();

            return MapList<Office, OfficeDto>(offices);
        }

        public IEnumerable<OfficeDto> GetOfficeSuggestions(SuggestionsRequest request)
        {
            IEnumerable<Office> offices = _officeRepository.AsEnumerable()
                .Where(x => x.MaxCapacity >= request.CapacityNeeded
                        && request.ResourcesNeeded.All(resource => x.AvailableResources.Contains(resource)))
                .OrderBy(x => x.Location.Neighborhood == request.PreferedNeigborHood ? 0 : 1)
                .ThenBy(x => x.MaxCapacity)
                .ThenBy(x => x.AvailableResources.Count());

            return MapList<Office, OfficeDto>(offices);
        }

        public List<TDestination> MapList<TSource, TDestination>(IEnumerable<TSource> sourceList)
        {
            var destinationList = new List<TDestination>();

            foreach (var source in sourceList)
                destinationList.Add(_mapper.Map<TDestination>(source));

            return destinationList;
        }
    }
}