using System;

namespace NetChallenge.Domain
{
    public class Booking
    {
        public int Id { get; private set; }
        public Office Office { get; private set; }
        public DateTime DateTime { get; private set; }
        public TimeSpan Duration { get; private set; }
        public string UserName { get; private set; }
    }
}