using Query_Gps.WebApi.Domain;

namespace Query_Gps.WebApi.Dtos
{
    public record VehicleGetDto(string? Type, string? Color, int Capacity)
    {
        public static implicit operator VehicleGetDto(Vehicle vehicle)
        {
            return new VehicleGetDto(vehicle.Type, vehicle.Color, vehicle.Capacity);
        }
    }
}
