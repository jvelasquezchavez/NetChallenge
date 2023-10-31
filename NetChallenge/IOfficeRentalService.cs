using NetChallenge.Dto.Input;
using NetChallenge.Dto.Output;
using System.Collections.Generic;

namespace NetChallenge
{
    public interface IOfficeRentalService
    {

        void AddLocation(AddLocationRequest request);

        void AddOffice(AddOfficeRequest request);

        void BookOffice(BookOfficeRequest request);

        IEnumerable<BookingDto> GetBookings(string locationName, string officeName);

        IEnumerable<LocationDto> GetLocations();

        IEnumerable<OfficeDto> GetOffices(string locationName);

        IEnumerable<OfficeDto> GetOfficeSuggestions(SuggestionsRequest request);
    }
}
