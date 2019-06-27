using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Authentication;
using NextDoor.Core.Common;
using NextDoor.Core.Dispatcher;
using NextDoor.Services.Identity.Messages.Commands;
using NextDoor.Services.Identity.Messages.Queries;
using NextDoor.Services.Identity.Services;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Controllers
{
    [Route("")]
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IIdentityService _identityService;
        private readonly IDispatcher _dispatcher;

        public IdentityController(IIdentityService identityService,
            IDispatcher dispatcher)
        {
            _identityService = identityService;
            _dispatcher = dispatcher;
        }

        [HttpGet("me")]
        [JwtAuth]
        public IActionResult Get() => Content($"Your id: '{UserId:N}'.");

        #region Sign-up and Sign-In using Dispatcher
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpCmd command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid");
            }

            await _dispatcher.SendAsync(command.BindId(c => c.Id));

            return NoContent();
        }

        [HttpPost("sign-in")]
        public async Task<ActionResult<JsonWebToken>> SignIn(SignInQuery query)
            => await _dispatcher.QueryAsync(query);
        #endregion

        #region Sign-up and Sign-In using Service
        [HttpPost("sign-up-svc")]
        public async Task<IActionResult> SignUpWithService(SignUpCmd dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid");
            }

            await _identityService.SignUpAsync(dto.Email,
                dto.Password, dto.Role);

            return NoContent();
        }

        [HttpPost("sign-in-svc")]
        public async Task<IActionResult> SignInWithService(SignInQuery dto)
            => Ok(await _identityService.SignInAsync(dto.Email, dto.Password));
        #endregion

        [HttpPut("change-pwd")]
        [JwtAuth]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto dto)
        {
            await _identityService.ChangePasswordAsync(dto.UserId,
                dto.CurrentPassword, dto.NewPassword);

            return NoContent();
        }
    }
}