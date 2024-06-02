namespace Query_Gps.WebApi.Dtos
{
    public record DriverSearchDto(double Latitude, double Longitude, double Radius, VehicleSearchDto Vehicle);
}
