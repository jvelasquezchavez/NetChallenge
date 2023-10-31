using NetChallenge.Domain;
using NetChallenge.Dto.Output;
using System.Linq;

namespace NetChallenge.Mappers
{
    public class OfficeMapper
    {
        public static Office MapToOffice(OfficeDto officeDto)
        {
            return new Office(new Location(), officeDto.Name, officeDto.MaxCapacity, officeDto.AvailableResources);
        }

        public static OfficeDto MapToOfficeDto(Office office)
        {
            OfficeDto officeDto = new OfficeDto();
            officeDto.Name = office.Name;
            officeDto.MaxCapacity = office.MaxCapacity;
            officeDto.AvailableResources = office.AvailableResources.ToArray<string>();
            officeDto.LocationName = office.Location.Name;

            return officeDto;
        }
    }
}
