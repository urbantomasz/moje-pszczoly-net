using MojePszczoly.Contracts.Responses;

namespace MojePszczoly.Interfaces
{
    public interface IBreadService
    {
        Task<List<BreadResponse>> GetBreads();
    }
}
