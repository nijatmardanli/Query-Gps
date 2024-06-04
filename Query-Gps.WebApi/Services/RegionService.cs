using Query_Gps.WebApi.Repositories.Abstract;
using Query_Gps.WebApi.Services.Abstract;

namespace Query_Gps.WebApi.Services
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        private readonly ReadAndQueryGeoJsonFileForStatePolygonService _polygonService;

        public RegionService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
            _polygonService = new(_regionRepository);
        }

        public async Task SyncronizeAsync()
        {
            await _polygonService.ReadGeoJsonDataWithDistrictAsync();
        }
    }
}
