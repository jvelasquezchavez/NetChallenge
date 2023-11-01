using NetChallenge.Domain;
using NetChallenge.Dto.Output;

namespace NetChallenge.Mappers
{
    public class BookingMapper
    {
        public static BookingDto MapToBookingDto(Booking booking)
        {
            BookingDto bookingDto = new BookingDto();
            bookingDto.LocationName = booking.Office.Location.Name;
            bookingDto.OfficeName = booking.Office.Name;
            bookingDto.Duration = booking.Duration;
            bookingDto.DateTime = booking.DateTime;
            bookingDto.UserName = booking.UserName;

            return bookingDto;
        }
    }
}
