using MojePszczoly.Domain.Entities;

namespace MojePszczoly.Application.Interfaces.Repositories
{
    public interface IBreadRepository
    {
        Task<List<Bread>> GetAllAsync();
        Task<List<Bread>> GetOrderedAsync();
    }
}