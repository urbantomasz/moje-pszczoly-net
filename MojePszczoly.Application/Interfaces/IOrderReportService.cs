namespace MojePszczoly.Application.Interfaces
{
    public interface IOrderReportService
    {
        Task<MemoryStream?> GetOrdersReportExcel(DateOnly date);
    }
}