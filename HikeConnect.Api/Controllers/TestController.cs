using Microsoft.AspNetCore.Mvc;

namespace HikeConnect.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTest()
        {
            return Ok();
        }
    }
}
