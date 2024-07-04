
using Query_Gps.WebApi.Dtos;

namespace Query_Gps.WebApi.Services.Abstract
{
    public interface IRegionService
    {
        Task<List<string>> FindAsync(RegionGetDto regionGetDto);
        Task SyncronizeAsync();
    }
}
