using Query_Gps.WebApi.Domain;

namespace Query_Gps.WebApi.Repositories.Abstract
{
    public interface IRegionRepository
    {
        Task UpsertAsync(Region entity);
    }
}
