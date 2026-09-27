using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MojePszczoly.Interfaces;
using MojePszczoly.Models;

namespace MojePszczoly.Controllers
{
    [Route("api/order-config")]
    [ApiController]
    [Authorize(Policy = "AllowedEmailsOnly")]
    public class OrderConfigController : ControllerBase
    {
        private readonly IOrderConfigService _service;

        public OrderConfigController(IOrderConfigService service)
        {
            _service = service;
        }

        /// <summary>Konfiguracja widoczna dla klienta skladajacego zamowienie.</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PublicOrderConfigDto>> Get(CancellationToken ct)
            => Ok(await _service.GetPublicAsync(ct));

        [HttpGet("admin")]
        public async Task<ActionResult<AdminOrderConfigDto>> GetForAdmin(CancellationToken ct)
            => Ok(await _service.GetForAdminAsync(ct));

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateOrderConfigRequest request, CancellationToken ct)
        {
            try
            {
                await _service.UpdateAsync(request, ct);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
