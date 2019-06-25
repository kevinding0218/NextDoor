using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Authentication;
using NextDoor.Services.Identity.Messages.Commands;
using NextDoor.Services.Identity.Services;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Controllers
{
    [Route("")]
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IIdentityService _identityService;
        //private readonly IDispatcher _dispatcher;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet("me")]
        [JwtAuth]
        public IActionResult Get() => Content($"Your id: '{UserId:N}'.");

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpCmd command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid");
            }

            await _identityService.SignUpAsync(command.Email,
                command.Password, command.Role);

            // await _dispatcher.SendAsync(command.BindId(c => c.Id));

            return NoContent();
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInCmd command)
            => Ok(await _identityService.SignInAsync(command.Email, command.Password));

        [HttpPut("change-pwd")]
        [JwtAuth]
        public async Task<ActionResult> ChangePassword(ChangePasswordCmd command)
        {
            await _identityService.ChangePasswordAsync(command.UserId,
                command.CurrentPassword, command.NewPassword);

            return NoContent();
        }
    }
}