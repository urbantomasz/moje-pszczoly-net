using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Data;
using MojePszczoly.Data.Models;
using MojePszczoly.Models;
using MojePszczoly.Services;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Globalization;

namespace MojePszczoly.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly DateService _dataService;

        public OrderController(AppDbContext context, DateService dateService, DateService dataService)
        {
            _context = context;
            _dataService = dataService;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 
 

            var order = new Order
            {
                CustomerName = orderDto.CustomerName,
                Phone = orderDto.Phone,
                OrderDate = orderDto.OrderDate,
                Note = orderDto.Note,
                Items = orderDto.Items.Select(itemDto => new OrderItem
                {
                    BreadId = itemDto.BreadId,
                    Quantity = itemDto.Quantity
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            return Ok(new { message = "Order created successfully!" });
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var dates = _dataService.GetUpcomingDates();

            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Bread)
                .AsNoTracking()
                .Where(o => dates.Select(x => x.Date).Contains(o.OrderDate.Date))
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            orders.ForEach(o =>
            {
                o.OrderDate = DateTime.SpecifyKind(o.OrderDate, DateTimeKind.Utc);
                o.CreatedAt = DateTime.SpecifyKind(o.CreatedAt, DateTimeKind.Utc);
            });

            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                Note = o.Note,
                Phone = o.Phone,
                CreatedAt = o.CreatedAt,
                OrderDate =  o.OrderDate,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    BreadId = i.BreadId,
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();

            var ordersDataDto = new OrdersDataDto
            {
                Orders = orderDtos,
                Dates = dates.ToList() 
            };

            return Ok(ordersDataDto);
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto updatedOrder)
        {
            var existingOrder = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (existingOrder == null)
                return NotFound();

            existingOrder.CustomerName = updatedOrder.CustomerName;
            existingOrder.Phone = updatedOrder.Phone;
            existingOrder.OrderDate = updatedOrder.OrderDate;
            existingOrder.Note = updatedOrder.Note;

            _context.OrderItems.RemoveRange(existingOrder.Items);

            existingOrder.Items = updatedOrder.Items
                .Select(item => new OrderItem
                {
                    BreadId = item.BreadId,
                    Quantity = item.Quantity,
                    OrderId = id 
                }).ToList();

            await _context.SaveChangesAsync();
            return Ok();
        }


        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet("report/excel/{date}")]
        public async Task<IActionResult> GetOrdersReportExcel(DateTime date)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Bread)
                .Where(o => o.OrderDate.Date == date.Date)
                .ToListAsync();

            if (!orders.Any())
                return NotFound(new { message = "Brak zamówień na ten dzień." });

            var allBreads = await _context.Breads
                .OrderBy(b => b.SortOrder)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Raport Zamówień");

            // 🔒 Zablokowanie wiersza nagłówków
            worksheet.View.FreezePanes(2, 1);

            // 🧠 Nagłówki
            worksheet.Cells[1, 1].Value = "KTO";
            worksheet.Cells[1, 2].Value = "KIEDY";
            for (int i = 0; i < allBreads.Count; i++)
            {
                worksheet.Cells[1, i + 3].Value = allBreads[i].ShortName;
            }

            // ✨ Formatowanie nagłówków
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
                var polishTime = TimeZoneInfo.ConvertTimeFromUtc(order.OrderDate, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
                worksheet.Cells[row, 2].Value = polishTime.ToString("dddd, dd.MM.yyyy", new CultureInfo("pl-PL"));



                for (int i = 0; i < allBreads.Count; i++)
                {
                    var bread = allBreads[i];
                    var item = order.Items.FirstOrDefault(x => x.BreadId == bread.BreadId);
                    int quantity = item?.Quantity ?? 0;
                    worksheet.Cells[row, i + 3].Value = quantity;
                }

                row++;
            }

            // 📊 Wiersz sumy
            worksheet.Cells[row, 1].Value = "SUMA";
            for (int i = 0; i < allBreads.Count; i++)
            {
                var col = i + 3;
                worksheet.Cells[row, col].Formula = $"SUM({worksheet.Cells[2, col].Address}:{worksheet.Cells[row - 1, col].Address})";
            }

            // 📦 Ramki wokół całej tabeli
            var dataRange = worksheet.Cells[1, 1, row, allBreads.Count + 2];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Raport_{date:yyyy-MM-dd}.xlsx"
            );
        }






    }
}
