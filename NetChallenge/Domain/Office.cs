using System.Collections.Generic;

namespace NetChallenge.Domain
{
    public class Office
    {
        public int Id { get; private set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public int MaxCapacity { get; set; }
        public IEnumerable<string> AvailableResources { get; set; }
    }
}