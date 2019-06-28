using Microsoft.AspNetCore.Mvc;

namespace NextDoor.Services.Notifications.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor Notification Service");

    }
}