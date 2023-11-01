using NetChallenge.Domain;
using NetChallenge.Dto.Output;

namespace NetChallenge.Mappers
{
    public class LocationMapper
    {
        public static LocationDto MapToLocationDto(Location location)
        {
            LocationDto locationDto = new LocationDto();
            locationDto.Name = location.Name;
            locationDto.Neighborhood = location.Neighborhood;

            return locationDto;
        }
    }
}
