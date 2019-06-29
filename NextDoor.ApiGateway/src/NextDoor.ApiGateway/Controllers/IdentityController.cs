using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextDoor.ApiGateway.Messages.Commands.Identity;
using NextDoor.ApiGateway.Messages.Queries.Identity;
using NextDoor.ApiGateway.Services;
using NextDoor.Core.Authentication;
using NextDoor.Core.Common;
using NextDoor.Core.RabbitMq;
using System.Threading.Tasks;

namespace NextDoor.ApiGateway.Controllers
{
    [Route("identity")]
    [ApiController]
    [JwtAuth]
    public class IdentityController : BaseController
    {
        private IIdentityService _identityService;
        public IdentityController(IBusPublisher busPublisher, IIdentityService identityService) : base(busPublisher)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Sign up command called first through our API Gateway and then published in rabbitmq with topic "identity"
        /// Then "Indentity" service will recieve it and execute "SignUpCommandHandler"
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("sign-up")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(SignUpCmd command)
            => await SendAsync(command.BindId(c => c.Id), resourceId: command.Id, resource: "identity");

        /// <summary>
        /// Redirect API Call to internal API Service
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInQuery query)
            => Single(await _identityService.SignInAsync(query));
    }
}