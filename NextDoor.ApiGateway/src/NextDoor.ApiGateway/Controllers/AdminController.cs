using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextDoor.ApiGateway.Messages.Queries;
using NextDoor.ApiGateway.Services;
using NextDoor.Core.RabbitMq;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Controllers
{
    [Route("admin")]
    [ApiController]
    [AllowAnonymous]
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
            => Collection(await _adminService.BrowseAsync(query));
    }
}