using Query_Gps.WebApi.Domain;

namespace Query_Gps.WebApi.Dtos
{
    public record DriverGetDto(Guid Id, Coordinates Coordinates, VehicleGetDto Vehicle)
    {
        public static implicit operator DriverGetDto(Driver driver)
        {
            return new DriverGetDto(driver.Id, driver.Coordinates, driver.Vehicle);
        }
    }
}
