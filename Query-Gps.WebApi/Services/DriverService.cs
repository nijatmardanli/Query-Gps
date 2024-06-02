using GeoJSON.Net.Geometry;
using Query_Gps.WebApi.Domain;
using Query_Gps.WebApi.Dtos;
using Query_Gps.WebApi.Repositories.Abstract;
using Query_Gps.WebApi.Services.Abstract;

namespace Query_Gps.WebApi.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;

        public DriverService(IDriverRepository driverRepository)
        {
            _driverRepository = driverRepository;
        }

        public async Task<List<DriverGetDto>> FindAsync(DriverSearchDto driverSearchDto)
        {
            Driver driverToSearch = new()
            {
                Coordinates = new Coordinates(driverSearchDto.Latitude, driverSearchDto.Longitude),
                Vehicle = new(driverSearchDto.Vehicle.Type, driverSearchDto.Vehicle.Color, driverSearchDto.Vehicle.Capacity)
            };

            List<Driver> drivers = await _driverRepository.FindAsync(driverToSearch, driverSearchDto.Radius);

            List<DriverGetDto> result = drivers.ConvertAll<DriverGetDto>(d => d);
            return result;
        }

        public async Task<Driver> AddAsync(DriverAddDto driverAddDto)
        {
            Driver driver = new()
            {
                Id = Guid.NewGuid(),
                Coordinates = new Coordinates(driverAddDto.Latitude, driverAddDto.Longitude),
                Vehicle = new Vehicle(driverAddDto.Vehicle.Type, driverAddDto.Vehicle.Color, driverAddDto.Vehicle.Capacity)
            };

            await _driverRepository.UpsertAsync(driver);

            return driver;
        }

        public async Task<Driver> UpdateCoordinateAsync(DriverUpdateCoordinateDto driverUpdateCoordinateDto)
        {
            var driver = await _driverRepository.GetAsync(driverUpdateCoordinateDto.Id);
            ArgumentNullException.ThrowIfNull(driver, nameof(driverUpdateCoordinateDto));

            driver.Coordinates = new Coordinates(driverUpdateCoordinateDto.Latitude, driverUpdateCoordinateDto.Longtitude);

            await _driverRepository.UpsertAsync(driver);

            return driver;
        }
    }
}
