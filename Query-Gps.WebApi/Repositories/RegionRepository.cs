using Query_Gps.WebApi.Domain;
using Query_Gps.WebApi.Repositories.Abstract;
using StackExchange.Redis;

namespace Query_Gps.WebApi.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        private const string _collection = "regions";

        public RegionRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        private IDatabase Database => _connectionMultiplexer.GetDatabase();

        public async Task<List<string>> FindAsync(Coordinates coordinates, double radius)
        {
            var data = await Database.ExecuteAsync("NEARBY",
                                                   _collection,
                                                   "Ids",
                                                   "POINT",
                                                   coordinates.Latitude,
                                                   coordinates.Longitude,
                                                   radius);


            List<string> result = new();

            if (data.Length < 2 || data[1].Length == 0)
                return result;

            for (int i = 0; i < data[1].Length; i++)
            {
                RedisResult item = data[1][i];
                result.Add(item.ToString());
            }

            return result;
        }

        public async Task UpsertAsync(Region entity)
        {
            _ = await Database.ExecuteAsync("SET",
                                            _collection,
                                            entity.Key,
                                            "OBJECT",
                                            entity.Value);
        }
    }
}
