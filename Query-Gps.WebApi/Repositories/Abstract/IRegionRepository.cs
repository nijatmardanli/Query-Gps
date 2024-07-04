using Query_Gps.WebApi.Domain;

namespace Query_Gps.WebApi.Repositories.Abstract
{
    public interface IRegionRepository
    {
        Task<List<string>> FindAsync(Coordinates coordinates, double radius);
        Task UpsertAsync(Region entity);
    }
}
