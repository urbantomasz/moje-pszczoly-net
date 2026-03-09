using MojePszczoly.Infrastructure.Entities;

namespace MojePszczoly.Interfaces
{
    public interface IDateOverrideService
    {
        Task AddExtraDay(DateOnly date);
        Task ExcludeDay(DateOnly date);
        Task<bool> RevertOverride(DateOnly date);
        Task<List<DateOverride>> GetOverrides();
    }
}
