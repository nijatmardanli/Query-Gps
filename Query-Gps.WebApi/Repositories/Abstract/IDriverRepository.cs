using Query_Gps.WebApi.Domain;

namespace Query_Gps.WebApi.Repositories.Abstract
{
    public interface IDriverRepository
    {
        Task<Driver?> GetAsync(Guid id);
        Task<List<Driver>> FindAsync(Driver entity, double radius);
        Task UpsertAsync(Driver entity);
    }
}
