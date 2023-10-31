namespace NetChallenge.Domain
{
    public class Location
    {
        public string Name { get; set; }
        public string Neighborhood { get; set; }
        public Location() { }

        public Location(string name, string neighborhood)
        {
            Name = name;
            Neighborhood = neighborhood;
        }
    }
}
