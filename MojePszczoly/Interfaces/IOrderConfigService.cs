using MojePszczoly.Models;

namespace MojePszczoly.Interfaces
{
    public interface IOrderConfigService
    {
        Task<PublicOrderConfigDto> GetPublicAsync(CancellationToken ct = default);

        Task<AdminOrderConfigDto> GetForAdminAsync(CancellationToken ct = default);

        Task UpdateAsync(UpdateOrderConfigRequest request, CancellationToken ct = default);
    }
}
