using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Dispatcher;

namespace NextDoor.Services.Admin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public BaseController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
    }
}
