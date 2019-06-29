using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Dispatcher;
using NextDoor.Services.Admin.Dto;
using NextDoor.Services.Admin.Messages.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDoor.Services.Admin.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public UsersController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> Get([FromQuery] BrowseUserQuery query)
            => Ok(await _dispatcher.QueryAsync(query));
    }
}