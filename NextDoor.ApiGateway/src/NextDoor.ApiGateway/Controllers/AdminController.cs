using Microsoft.AspNetCore.Mvc;
using NextDoor.ApiGateway.Messages.Queries.Admin;
using NextDoor.ApiGateway.Services;
using NextDoor.Core.Authentication;
using NextDoor.Core.RabbitMq;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Controllers
{
    [Route("admin")]
    [ApiController]
    [AdminAuth]
    public class AdminController : BaseController
    {
        private readonly IAdminService _adminService;

        public AdminController(IBusPublisher busPublisher, IAdminService adminService)
            : base(busPublisher)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Redirect API Call to internal API Service
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] BrowseUserQuery query)
            => Collection(await _adminService.BrowseUsersAsync(query));
    }
}