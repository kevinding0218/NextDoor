using Microsoft.AspNetCore.Mvc;

namespace NextDoor.Services.Identity.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor Identity Service");

    }
}