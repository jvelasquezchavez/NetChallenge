using NetChallenge.Abstractions;
using NetChallenge.Domain;
using System;
using System.Collections.Generic;

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
    }
}