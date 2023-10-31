using System;

namespace NetChallenge.Exceptions
{
    public class OfficeRentalException : Exception
    {
        public OfficeRentalException()
        {
        }

        public OfficeRentalException(string message)
            : base(message)
        {
        }

        public OfficeRentalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public class LocationNotFoundException : OfficeRentalException
        {
            public LocationNotFoundException(string locationName)
                : base($"Location not found: '{locationName}'")
            {
            }
        }

        public class OfficeNotFoundException : OfficeRentalException
        {
            public OfficeNotFoundException(string officeName)
                : base($"Office not found: '{officeName}'")
            {
            }
        }

        // Excepciones relacionadas con Location
        public class LocationNameNullOrEmptyException : OfficeRentalException
        {
            public LocationNameNullOrEmptyException() : base("Location name cannot be empty.")
            {
            }
        }

        public class LocationNeighborhoodNullOrEmptyException : OfficeRentalException
        {
            public LocationNeighborhoodNullOrEmptyException() : base("Neighborhood cannot be empty.")
            {
            }
        }

        public class LocationNameDuplicateException : OfficeRentalException
        {
            public LocationNameDuplicateException(string locationName) : base($"Location '{locationName}' already exists.")
            {
            }
        }

        public class OfficeHasSameNameAsLocationException : OfficeRentalException
        {
            public OfficeHasSameNameAsLocationException() : base("The location and the office has the same name.")
            {
            }
        }

        // Excepciones relacionadas con Office
        public class InvalidLocationException : OfficeRentalException
        {
            public InvalidLocationException() : base("Office location is invalid.")
            {
            }
        }

        public class OfficeNameNullOrEmptyException : OfficeRentalException
        {
            public OfficeNameNullOrEmptyException() : base("Office name cannot be empty.")
            {
            }
        }

        public class OfficeNameDuplicateException : OfficeRentalException
        {
            public OfficeNameDuplicateException() : base("Office name already exists in this location.")
            {
            }
        }

        public class InvalidCapacityException : OfficeRentalException
        {
            public InvalidCapacityException() : base("Office maximum capacity must be greater than zero.")
            {
            }
        }

        // Excepciones relacionadas con Booking
        public class InvalidOfficeException : OfficeRentalException
        {
            public InvalidOfficeException() : base("Booking office is invalid.")
            {
            }
        }

        public class InvalidDurationException : OfficeRentalException
        {
            public InvalidDurationException() : base("Booking duration must be greater than zero.")
            {
            }
        }

        public class BookingConflictException : OfficeRentalException
        {
            public BookingConflictException() : base("Booking conflict detected.")
            {                
            }
        }

        public class BookingDateTimeConflictException : OfficeRentalException
        {
            public BookingDateTimeConflictException() : base("Date Booking conflict detected.")
            {
            }
        }

        public class UserRequiredException : OfficeRentalException
        {
            public UserRequiredException() : base("User name is required.")
            {
            }
        }

        public class InvalidDurationInHoursException : OfficeRentalException
        {
            public InvalidDurationInHoursException() : base("Duration must be in whole hours.")
            {
            }
        }

    }
}
