using Microsoft.AspNetCore.Mvc;

namespace ErrorHandling.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SimplesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GeraExceptionDeTeste()
        {
            throw new Exception("Gerando uma Exception no controller");
        }
    }
}
