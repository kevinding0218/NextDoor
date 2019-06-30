using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NextDoor.ApiGateway.Controllers
{
    [Route("")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor Api Gateway");
    }
}