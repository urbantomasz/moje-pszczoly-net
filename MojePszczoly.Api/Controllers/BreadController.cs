using Microsoft.AspNetCore.Mvc;
using MojePszczoly.Contracts.Responses;
using MojePszczoly.Application.Interfaces;

namespace MojePszczoly.Api.Controllers
{
    [Route("api/breads")]
    [ApiController]
    public class BreadController : ControllerBase
    {
        private readonly IBreadService _breadService;

        public BreadController(IBreadService breadService)
        {
            _breadService = breadService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<BreadResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BreadResponse>>> GetBreads()
        {
            return Ok(await _breadService.GetBreads());
        }
    }
}
