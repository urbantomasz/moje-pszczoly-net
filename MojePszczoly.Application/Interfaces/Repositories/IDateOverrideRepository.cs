using MojePszczoly.Domain.Entities;

namespace MojePszczoly.Application.Interfaces.Repositories
{
    public interface IDateOverrideRepository
    {
        Task<DateOverride?> GetByDateAsync(DateOnly date);
        Task AddAsync(DateOverride overrideEntry);
        Task<bool> DeleteAsync(DateOverride overrideEntry);
        Task<List<DateOverride>> GetFromDateAsync(DateOnly date);
        Task<List<DateOverride>> GetBetweenDatesAsync(DateOnly startDate, DateOnly endDate);
    }
}