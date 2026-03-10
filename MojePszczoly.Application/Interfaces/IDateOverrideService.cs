using MojePszczoly.Domain.Entities;

namespace MojePszczoly.Application.Interfaces
{
    public interface IDateOverrideService
    {
        Task AddExtraDay(DateOnly date);
        Task ExcludeDay(DateOnly date);
        Task<bool> RevertOverride(DateOnly date);
        Task<List<DateOverride>> GetOverrides();
    }
}
