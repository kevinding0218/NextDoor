using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NextDoor.Services.Identity.Controllers
{
    [Route("")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor Identity Service");

    }
}