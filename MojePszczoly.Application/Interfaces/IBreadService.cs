using MojePszczoly.Contracts.Responses;

namespace MojePszczoly.Application.Interfaces
{
    public interface IBreadService
    {
        Task<List<BreadResponse>> GetBreads();
    }
}
