using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojePszczoly.Contracts.Requests;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Application.Interfaces;


namespace MojePszczoly.Api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IOrderReportService _orderReportService;

        public OrderController(IOrderService orderService, IOrderReportService orderReportService)
        {
            _orderService = orderService;
            _orderReportService = orderReportService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.CreateOrder(request);

            return Created(string.Empty, new { message = "Order created successfully!" });
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet]
        public async Task<ActionResult<List<OrderResponse>>> GetOrders()
        {
            var orders = await _orderService.GetOrders();

            return Ok(orders);
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet("history")]
        public async Task<ActionResult<List<OrderResponse>>> GetPastOrders()
        {
            var orders = await _orderService.GetPastOrders();
            return Ok(orders);
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id);

            if (!result)
                return NotFound();

            return NoContent();
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOrder(int id, [FromBody] UpdateOrderRequest request)
        {
            var result = await _orderService.UpdateOrder(id, request);

            if (!result)
                return NotFound();

            return Ok();
        }

        [Authorize(Policy = "AllowedEmailsOnly")]
        [HttpGet("report/excel/{date}")]
        public async Task<ActionResult> GetOrdersReportExcel(DateOnly date)
        {
            var stream = await _orderReportService.GetOrdersReportExcel(date);

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
