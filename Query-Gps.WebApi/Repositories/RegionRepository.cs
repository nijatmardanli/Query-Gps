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
