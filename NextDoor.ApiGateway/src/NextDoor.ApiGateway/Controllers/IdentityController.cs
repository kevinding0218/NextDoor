using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextDoor.ApiGateway.Messages.Commands.Identity;
using NextDoor.Core.Common;
using NextDoor.Core.RabbitMq;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Controllers
{
    [Route("")]
    [ApiController]
    [AllowAnonymous]
    public class IdentityController : BaseController
    {
        public IdentityController(IBusPublisher busPublisher) : base(busPublisher)
        {
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> Post(SignUpCmd command)
            => await SendAsync(command.BindId(c => c.Id), resourceId: command.Id, resource: "identity");
    }
}