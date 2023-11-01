using System.Collections.Generic;

namespace NetChallenge.Domain
{
    public class Location
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Neighborhood { get; set; }
        public ICollection<Office> offices { get; set; }

        public Location() 
        {
            offices = new HashSet<Office>();
        }
    }
}
