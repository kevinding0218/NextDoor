using Microsoft.AspNetCore.Mvc;

namespace NextDoor.Services.Customers.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor Admin Service");

    }
}