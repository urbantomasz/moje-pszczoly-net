using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Data;
using MojePszczoly.Data.Models;
using MojePszczoly.Models;

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

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Bread)
                .ToListAsync();

            var orderDtos = orders.Select(o => new OrderDto
            {
                OrderId = o.OrderId,
                CustomerName = o.CustomerName,
                Note = o.Note,
                Phone = o.Phone,
                OrderDate = o.OrderDate,
                Items = o.Items.Select(i => new OrderItemDto
                {
                    OrderItemId = i.OrderItemId,
                    BreadId = i.BreadId,
                    BreadName = i.Bread.ShortName,
                    Quantity = i.Quantity
                }).ToList()
            }).ToList();

            return Ok(orderDtos);
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

        [HttpGet("{date}")]
        public async Task<IActionResult> GetOrdersByDate(DateTime date)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Bread)
                .Where(o => o.OrderDate.Date == date.Date)
                .ToListAsync();

            if (!orders.Any())
                return NotFound(new { message = "Brak zamówień na ten dzień." });

            var groupedOrders = orders.Select(o => new
            {
                o.CustomerName,
                Items = o.Items.Select(i => new { i.Bread.Name, i.Quantity }).ToList()
            });

            return Ok(new
            {
                Date = date.ToString("dd.MM.yyyy"),
                Orders = groupedOrders
            });
        }

        [HttpGet("report/{date}")]
        public async Task<IActionResult> GetOrdersReport(DateTime date)
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Bread)
                .Where(o => o.OrderDate.Date == date.Date)
                .ToListAsync();

            if (!orders.Any())
                return NotFound(new { message = "Brak zamówień na ten dzień." });

            // Pobranie unikalnych typów chleba (nagłówki kolumn)
            var breadTypes = _context.Breads.Select(b => b.Name).ToList();

            // Lista do przechowywania tabeli
            var table = new List<Dictionary<string, object>>();

            // Inicjalizacja wiersza sumującego
            var totalRow = new Dictionary<string, object>
            {
                { "Data", $"{date:dd.MM.yyyy} Total" },
                { "Kto", "Razem" }
            };

            // Inicjalizacja sumy dla każdego chleba
            foreach (var bread in breadTypes)
                totalRow[bread] = 0;

            // Dodanie zamówień do tabeli
            foreach (var order in orders)
            {
                var row = new Dictionary<string, object>
                {
                    { "Data", "" },
                    { "Kto", order.CustomerName }
                };

                // Inicjalizacja wartości chleba na 0
                foreach (var bread in breadTypes)
                    row[bread] = 0;

                // Wypełnianie tabeli danymi
                foreach (var item in order.Items)
                {
                    if (row.ContainsKey(item.Bread.Name))
                    {
                        row[item.Bread.Name] = item.Quantity;
                        totalRow[item.Bread.Name] = (int)totalRow[item.Bread.Name] + item.Quantity;
                    }
                }

                table.Add(row);
            }

            // Dodanie wiersza sumującego dla tego dnia
            table.Add(totalRow);

            // Grand Total - sumuje wszystkie zamówienia w historii
            var grandTotalRow = new Dictionary<string, object>
            {
                { "Data", "Grand Total" },
                { "Kto", "Suma wszystkich dni" }
            };

            foreach (var bread in breadTypes)
            {
                grandTotalRow[bread] = await _context.OrderItems
                    .Where(i => i.Bread.Name == bread)
                    .SumAsync(i => i.Quantity);
            }

            // Dodanie Grand Total na końcu tabeli
            table.Add(grandTotalRow);

            return Ok(table);
        }


    }
}
