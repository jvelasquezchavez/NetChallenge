using System;

namespace NetChallenge.Domain
{
    public class Booking
    {
        public Guid Id { get; private set; }
        public Office Office { get; private set; }
        public DateTime DateTime { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string User { get; private set; }

        private Booking() { } // Constructor privado para Entity Framework u otras herramientas ORM

        public Booking(string locationName, string officeName, DateTime dateTime, TimeSpan duration, string user)
        {
            if (officeName == null)
                throw new ArgumentNullException("La oficina de la reserva no puede ser nula.");
            if (duration <= TimeSpan.Zero)
                throw new ArgumentException("La duración de la reserva debe ser mayor a cero.");
            //if (office.HasOverlap(dateTime, duration))
            //    throw new InvalidOperationException("La reserva se superpone con otras reservas en la misma oficina.");
            if (user == null)
                throw new ArgumentNullException("El usuario de la reserva no puede ser nulo.");

            Id = Guid.NewGuid();
            Office = new Office(new Location(locationName, ""), officeName, 1, null);
            DateTime = dateTime;
            Duration = duration;
            User = user;
        }
    }
}