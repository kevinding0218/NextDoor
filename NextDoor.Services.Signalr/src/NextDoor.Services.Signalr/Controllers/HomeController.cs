using Microsoft.AspNetCore.Mvc;

namespace NextDoor.Services.Signalr.Controllers
{
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor SignalR Service");
    }
}
