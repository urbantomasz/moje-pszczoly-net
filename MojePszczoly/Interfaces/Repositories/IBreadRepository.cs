using MojePszczoly.Infrastructure.Entities;

namespace MojePszczoly.Interfaces.Repositories
{
    public interface IBreadRepository
    {
        Task<List<Bread>> GetAllAsync();
        Task<List<Bread>> GetOrderedAsync();
    }
}