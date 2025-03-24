using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MojePszczoly.Services;

namespace MojePszczoly.Controllers
{
    [Route("api/dates")]
    [ApiController]
    public class DateController : ControllerBase
    {
        private readonly DateService _dataService;
        public DateController(DateService dataService)
        {
            _dataService = dataService;
        }
        [HttpGet]
        public IActionResult GetUpcomingDates()
        {
            return Ok(_dataService.GetUpcomingDates());
        }
    }
}
