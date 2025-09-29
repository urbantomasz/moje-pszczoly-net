using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojePszczoly.Data;
using MojePszczoly.Interfaces;
using MojePszczoly.Models;
using MojePszczoly.Services;

namespace MojePszczoly.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _orderService.CreateOrder(orderDto);
            return Ok(new { message = "Order created successfully!" });
        }



        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders();
            return Ok(orders);
        }


        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet("date")]
        public async Task<IActionResult> GetOrders(DateTime dateTime)
        {
            var orders = await _orderService.GetOrders(dateTime);
            return Ok(orders);
        }



        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet("history")]
        public async Task<IActionResult> GetPastOrders()
        {
            var orders = await _orderService.GetPastOrders();
            return Ok(orders);
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);
            if (!result)
                return NotFound();

            return Ok();
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto updatedOrder)
        {
            var result = await _orderService.UpdateOrder(id, updatedOrder);
            if (!result)
                return NotFound();

            return Ok();
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet("report/excel/{date}")]
        public async Task<IActionResult> GetOrdersReportExcel(DateTime date)
        {
            var stream = await _orderService.GetOrdersReportExcel(date);
            if (stream == null)
                return NotFound(new { message = "Brak zamówień na ten dzień." });

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Raport_{date:yyyy-MM-dd}.xlsx"
            );
        }
    }
}
