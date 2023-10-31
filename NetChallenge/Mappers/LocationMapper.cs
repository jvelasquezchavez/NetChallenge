using NetChallenge.Domain;
using NetChallenge.Dto.Output;

namespace NetChallenge.Mappers
{
    public class LocationMapper
    {
        public static Location MapToLocation(LocationDto locationDto) 
            => new Location(locationDto.Name, locationDto.Neighborhood);

        public static LocationDto MapToLocationDto(Location location)
        {
            LocationDto locationDto = new LocationDto();
            locationDto.Name = location.Name;
            locationDto.Neighborhood = location.Neighborhood;

            return locationDto;
        }
    }
}
