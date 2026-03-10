using Microsoft.AspNetCore.Mvc;
using MojePszczoly.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MojePszczoly.Domain.Entities;

namespace MojePszczoly.Api.Controllers
{
    [ApiController]
    [Authorize(Policy = "AllowedEmailsOnly")]
    [Route("api/[controller]")]
    public class DateOverrideController : ControllerBase
    {
        private readonly IDateOverrideService _service;

        public DateOverrideController(IDateOverrideService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<DateOverride>>> GetOverrides()
        {
            var overrides = await _service.GetOverrides();
            return Ok(overrides);
        }

        [HttpPost("extra")]
        public async Task<IActionResult> AddExtraDay([FromQuery] DateOnly date)
        {
            await _service.AddExtraDay(date);
            return NoContent();
        }

        [HttpPost("exclude")]
        public async Task<IActionResult> ExcludeDay([FromQuery] DateOnly date)
        {
            await _service.ExcludeDay(date);
            return NoContent();
        }

        [HttpPost("revert")]
        public async Task<ActionResult<bool>> RevertOverride([FromQuery] DateOnly date)
        {
            var result = await _service.RevertOverride(date);
            if (result) return Ok(true);
            return NotFound(false);
        }
    }
}
