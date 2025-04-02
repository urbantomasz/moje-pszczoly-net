using Microsoft.AspNetCore.Mvc;
using MojePszczoly.Data;
using MojePszczoly.Interfaces;

namespace MojePszczoly.Controllers
{
    [Route("api/breads")]
    [ApiController]
    public class BreadController : ControllerBase
    {
        private readonly IBreadService _breadService;

        public BreadController(AppDbContext context, IBreadService breadService)
        {
            _breadService = breadService;
        }

        [HttpGet]
        public IActionResult GetBreads()
        {
            return Ok(_breadService.GetBreads());
        }
    }
}
