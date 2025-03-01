using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Data;
using MojePszczoly.Data.Models;
using MojePszczoly.Models;
using OfficeOpenXml;

namespace MojePszczoly.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
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
                OrderDate = orderDto.OrderDate.Date,
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

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Bread)
                .AsNoTracking()
                .OrderBy(o => o.OrderDate)
                .ToListAsync();

            var dates = orders.Select(o => o.OrderDate.Date).Distinct();

            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                Note = o.Note,
                Phone = o.Phone,
                OrderDate = o.OrderDate.Date,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    OrderItemId = i.OrderItemId,
                    BreadId = i.BreadId,
                    BreadName = i.Bread.ShortName,
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] Order updatedOrder)
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

            existingOrder.Items = updatedOrder.Items;

            await _context.SaveChangesAsync();
            return Ok();
        }

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

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Raport Zamówień");

            // Nagłówki kolumn
            worksheet.Cells[1, 1].Value = "Data";
            worksheet.Cells[1, 2].Value = "Kto";
            worksheet.Cells[1, 3].Value = "Telefon";
            worksheet.Cells[1, 4].Value = "Chleby";
            worksheet.Cells[1, 5].Value = "Uwagi";

            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cells[row, 1].Value = order.OrderDate.ToString("yyyy-MM-dd");
                worksheet.Cells[row, 2].Value = order.CustomerName;
                worksheet.Cells[row, 3].Value = order.Phone;
                worksheet.Cells[row, 4].Value = string.Join(", ", order.Items.Select(i => $"{i.Bread.Name} ({i.Quantity})"));
                worksheet.Cells[row, 5].Value = order.Note;
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Raport_{date:yyyy-MM-dd}.xlsx");
        }



    }
}
