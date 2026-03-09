namespace MojePszczoly.Interfaces
{
    public interface IOrderReportService
    {
        Task<MemoryStream?> GetOrdersReportExcel(DateOnly date);
    }
}