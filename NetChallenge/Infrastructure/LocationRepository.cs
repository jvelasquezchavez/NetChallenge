using System;
using System.Collections.Generic;
using System.Linq;
using NetChallenge.Abstractions;
using NetChallenge.Domain;

namespace NetChallenge.Infrastructure
{
    public class LocationRepository : ILocationRepository
    {
        private readonly List<Location> _locations;

        public LocationRepository()
        {
            _locations = new List<Location>();
        }

        public IEnumerable<Location> AsEnumerable() => _locations; 

        public void Add(Location item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _locations.Add(item);
        }

        public Location GetLocationByName(string name) => _locations.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}