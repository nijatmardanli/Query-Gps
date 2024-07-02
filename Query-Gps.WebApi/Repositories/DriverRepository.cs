using Query_Gps.WebApi.Domain;
using Query_Gps.WebApi.Repositories.Abstract;
using StackExchange.Redis;
using System.Text.Json;

namespace Query_Gps.WebApi.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        private const string _collection = "drivers";

        public DriverRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        private IDatabase Database => _connectionMultiplexer.GetDatabase();

        public async Task<Driver?> GetAsync(Guid id)
        {
            RedisResult result = await Database.ExecuteAsync("Get", _collection, id.ToString(), "Withfields", "Object");
            if (result[1].Length <= 0)
                return null!;

            Driver driver = JsonSerializer.Deserialize<Driver>(result[1][1].ToString())!;

            return driver;
        }

        public async Task<List<Driver>> FindAsync(Driver entity, double radius)
        {
            var coordinates = entity.Coordinates;

            string condition = $"Data.Vehicle.Type==\"{entity.Vehicle.Type}\" && Data.Vehicle.Capacity=={entity.Vehicle.Capacity} && Data.Vehicle.Color==\"{entity.Vehicle.Color}\"";
            RedisResult result = await Database.ExecuteAsync("Nearby",
                                                             _collection,
                                                             "Where",
                                                             condition,
                                                             "Point",
                                                             coordinates.Latitude,
                                                             coordinates.Longitude,
                                                             radius);


            List<Driver> drivers = new(capacity: result[1].Length);

            for (var i = 0; i < result[1].Length; i++)
            {
                string driverResult = result[1][i][2][1].ToString();
                //string coordinatesResult = result[1][i][1].ToString();

                Driver driver = JsonSerializer.Deserialize<Driver>(driverResult)!;
                //JsonNode? coordinatesNode = JsonNode.Parse(coordinatesResult);

                //double[] coordinatesArray = JsonSerializer.Deserialize<double[]>(coordinatesNode!["coordinates"])!;

                //driver.Coordinates = new Point(new Position(coordinatesArray[1], coordinatesArray[0]));

                drivers.Add(driver);
            }

            return drivers;
        }

        public async Task UpsertAsync(Driver entity)
        {
            string json = JsonSerializer.Serialize(entity);

            var coordinates = entity.Coordinates;

            _ = await Database.ExecuteAsync("Set",
                                            _collection,
                                            entity.Id.ToString(),
                                            "Field",
                                            "Data",
                                            json,
                                            "Point",
                                            coordinates.Latitude,
                                            coordinates.Longitude);
        }
    }
}
