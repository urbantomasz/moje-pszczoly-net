using Microsoft.AspNetCore.Mvc;
using MojePszczoly.Data;

namespace MojePszczoly.Controllers
{
    [Route("api/breads")]
    [ApiController]
    public class BreadController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BreadController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetBreads()
        {
            return Ok(_context.Breads.ToList());
        }
    }
}
