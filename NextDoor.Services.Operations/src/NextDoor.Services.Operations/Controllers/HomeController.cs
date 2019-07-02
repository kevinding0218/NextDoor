using Microsoft.AspNetCore.Mvc;

namespace NextDoor.Services.Operations.Controllers
{
    [Route("")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor Operations Service");
    }
}
