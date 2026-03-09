using Microsoft.AspNetCore.Mvc;  
using MojePszczoly.Interfaces;

namespace MojePszczoly.Controllers
{
    [Route("api/dates")]
    [ApiController]
    public class DateController : ControllerBase
    {
        private readonly IDateService _dataService;

        public DateController(IDateService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<DateOnly>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<DateOnly>>> GetUpcomingDates()
        {
            return Ok(await _dataService.GetUpcomingDates());
        }

        [HttpGet("current")]
        [ProducesResponseType(typeof(List<DateOnly>), StatusCodes.Status200OK)]
        public ActionResult<List<DateOnly>> GetCurrentWeekDates()
        {
            return Ok(_dataService.GetCurrentWeekDates());
        }
    }
}
