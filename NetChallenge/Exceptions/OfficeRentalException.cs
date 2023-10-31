using System;
using System.Collections.Generic;
using System.Text;

namespace NetChallenge.Exceptions
{
    public class OfficeRentalException: Exception
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
                : base($"La ubicación '{locationName}' no se encontró.")
            {
            }
        }

        public class OfficeNotFoundException : OfficeRentalException
        {
            public OfficeNotFoundException(string officeName)
                : base($"La oficina '{officeName}' no se encontró.")
            {
            }
        }

        // Excepciones relacionadas con Location
        public class LocationNameNullOrEmptyException : OfficeRentalException
        {
            public LocationNameNullOrEmptyException() : base("El nombre de la ubicación no puede estar vacío.")
            {
            }
        }

        public class LocationNeighborhoodNullOrEmptyException : OfficeRentalException
        {
            public LocationNeighborhoodNullOrEmptyException() : base("El barrio de la ubicación no puede estar vacío.")
            {
            }
        }

        public class LocationNameDuplicateException : OfficeRentalException
        {
            public LocationNameDuplicateException() : base("El nombre de la ubicación ya existe.")
            {
            }
        }
        
        public class OfficeHasSameNameAsLocationException : OfficeRentalException
        {
            public OfficeHasSameNameAsLocationException() : base("The location ya existe.")
            {
            }
        }

        // Excepciones relacionadas con Office
        public class InvalidLocationException : OfficeRentalException
        {
            public InvalidLocationException() : base("La ubicación de la oficina no es válida.")
            {
            }
        }

        public class OfficeNameNullOrEmptyException : OfficeRentalException
        {
            public OfficeNameNullOrEmptyException() : base("El nombre de la oficina no puede estar vacío.")
            {
            }
        }

        public class OfficeNameDuplicateException : OfficeRentalException
        {
            public OfficeNameDuplicateException() : base("El nombre de la oficina ya existe en esta ubicación.")
            {
            }
        }

        public class InvalidCapacityException : OfficeRentalException
        {
            public InvalidCapacityException() : base("La capacidad máxima de la oficina debe ser mayor a cero.")
            {
            }
        }

        // Excepciones relacionadas con Booking
        public class InvalidOfficeException : OfficeRentalException
        {
            public InvalidOfficeException() : base("La oficina de la reserva no es válida.")
            {
            }
        }

        public class InvalidDurationException : OfficeRentalException
        {
            public InvalidDurationException() : base("La duración de la reserva debe ser mayor a cero.")
            {
            }
        }

        public class BookingConflictException : OfficeRentalException
        {
            public BookingConflictException() : base("La reserva se superpone con otras reservas de la misma oficina.")
            {
            }
        }

        public class UserRequiredException : OfficeRentalException
        {
            public UserRequiredException() : base("El usuario que realiza la reserva es obligatorio.")
            {
            }
        }
    }
}
