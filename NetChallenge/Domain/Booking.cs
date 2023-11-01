using System;

namespace NetChallenge.Domain
{
    public class Booking
    {
        public int Id { get; set; }
        public Office Office { get; set; }
        public DateTime DateTime { get; set; }
        public TimeSpan Duration { get; private set; }
        public string UserName { get; set; }
    }
}