using System.Collections.Generic;
using System;

namespace NetChallenge.Domain
{
    public class Office
    {
        public Guid Id { get; private set; }
        public Location Location { get; private set; }
        public string Name { get; private set; }
        public int MaxCapacity { get; private set; }
        public IEnumerable<string> AvailableResources { get; private set; }

        private Office() { } // Constructor privado para Entity Framework u otras herramientas ORM

        public Office(Location location, string name, int maxCapacity, IEnumerable<string> availableResources)
        {
            if (location == null)
                throw new ArgumentNullException("La ubicación de la oficina no puede ser nula.");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("El nombre de la oficina no puede estar vacío.");
            if (maxCapacity <= 0)
                throw new ArgumentException("La capacidad máxima debe ser mayor a cero.");

            Id = Guid.NewGuid();
            Location = location;
            Name = name;
            MaxCapacity = maxCapacity;
            AvailableResources = availableResources ?? new List<string>();
        }
    }
}