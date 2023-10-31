using System;
using System.Collections.Generic;
using System.Linq;
using NetChallenge.Abstractions;
using NetChallenge.Domain;

namespace NetChallenge.Infrastructure
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly List<Office> _offices;

        public OfficeRepository()
        {
            _offices = new List<Office>();
        }

        public IEnumerable<Office> AsEnumerable() => _offices;

        public void Add(Office item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _offices.Add(item);
        }

        public Office GetOfficeByNameAndLocation(string name, Location location) => 
            _offices.FirstOrDefault(office =>
                office.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && office.Location == location);

        public IEnumerable<Office> GetOfficeByLocation(string locationName) =>
            _offices.Where(office => office.Location.Name == locationName);
    }
}