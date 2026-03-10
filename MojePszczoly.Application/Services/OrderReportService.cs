using MojePszczoly.Application.Interfaces;
using MojePszczoly.Application.Interfaces.Repositories;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace MojePszczoly.Application.Services
{
    public class OrderReportService : IOrderReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBreadRepository _breadRepository;

        public OrderReportService(IOrderRepository orderRepository, IBreadRepository breadRepository)
        {
            _orderRepository = orderRepository;
            _breadRepository = breadRepository;
        }

        public async Task<MemoryStream?> GetOrdersReportExcel(DateOnly date)
        {
            var orders = await _orderRepository.GetByDateAsync(date);

            if (!orders.Any())
                return null;

            var allBreads = await _breadRepository.GetOrderedAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Raport Zamówień");

            // Zablokowanie wiersza nagłówków
            worksheet.View.FreezePanes(2, 1);

            // Nagłówki
            worksheet.Cells[1, 1].Value = "KTO";
            worksheet.Cells[1, 2].Value = "KIEDY";
            for (int i = 0; i < allBreads.Count; i++)
            {
                worksheet.Cells[1, i + 3].Value = allBreads[i].ShortName;
            }

            // Formatowanie nagłówków
            using (var headerRange = worksheet.Cells[1, 1, 1, allBreads.Count + 2])
            {
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                headerRange.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            }

            int row = 2;

            foreach (var order in orders)
            {
                worksheet.Cells[row, 1].Value = order.CustomerName;
                worksheet.Cells[row, 2].Value = order.OrderDate.ToString("dddd, dd.MM.yyyy", new CultureInfo("pl-PL"));

                for (int i = 0; i < allBreads.Count; i++)
                {
                    var bread = allBreads[i];
                    var item = order.Items.FirstOrDefault(x => x.BreadId == bread.BreadId);
                    int quantity = item?.Quantity ?? 0;
                    worksheet.Cells[row, i + 3].Value = quantity;
                }

                row++;
            }

            // Wiersz sumy
            worksheet.Cells[row, 1].Value = "SUMA";
            for (int i = 0; i < allBreads.Count; i++)
            {
                var col = i + 3;
                worksheet.Cells[row, col].Formula = $"SUM({worksheet.Cells[2, col].Address}:{worksheet.Cells[row - 1, col].Address})";
            }

            // 👉 suma wszystkich chlebów w kolumnie 2 ("KIEDY")
            var firstBreadCol = 3;
            var lastBreadCol = allBreads.Count + 2;
            worksheet.Cells[row, 2].Formula =
                $"SUM({worksheet.Cells[row, firstBreadCol].Address}:{worksheet.Cells[row, lastBreadCol].Address})";

            // Ramki wokół całej tabeli
            var dataRange = worksheet.Cells[1, 1, row, allBreads.Count + 2];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }
    }
}
