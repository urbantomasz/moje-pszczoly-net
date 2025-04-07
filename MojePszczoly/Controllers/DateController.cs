using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Interfaces;
using MojePszczoly.Services;

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
        public IActionResult GetUpcomingDates()
        {
            return Ok(_dataService.GetUpcomingDates());
        }

        [HttpGet("past")]
        public IActionResult GetPastDates()
        {
            return Ok(_dataService.GetPastDates());
        }

        [HttpGet("current")]
        public IActionResult GetCurrentWeekDates()
        {
            return Ok(_dataService.GetCurrentWeekDates());
        }
    }
}
