using Query_Gps.WebApi.Domain;
using Query_Gps.WebApi.Dtos;

namespace Query_Gps.WebApi.Services.Abstract
{
    public interface IDriverService
    {
        Task<Driver> AddAsync(DriverAddDto driverAddDto);
        Task<List<DriverGetDto>> FindAsync(DriverSearchDto driverSearchDto);
        Task<Driver> UpdateCoordinateAsync(DriverUpdateCoordinateDto driverUpdateCoordinateDto);
    }
}
