using NetChallenge.Domain;
using NetChallenge.Dto.Output;

namespace NetChallenge.Mappers
{
    public class BookingMapper
    {
        public static Booking MapToLocation(BookingDto bookingDto)
            => new Booking(bookingDto.LocationName, bookingDto.OfficeName, bookingDto.DateTime, bookingDto.Duration,bookingDto.UserName);

        public static BookingDto MapToLocationDto(Booking booking)
        {
            BookingDto bookingDto = new BookingDto();

            return bookingDto;
        }
    }
}
