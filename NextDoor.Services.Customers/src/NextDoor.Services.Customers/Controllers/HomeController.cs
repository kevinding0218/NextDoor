using Microsoft.AspNetCore.Mvc;

namespace NextDoor.Services.Customers.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Get() => Ok("NextDoor Customer Service");

    }
}