using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Authentication;
using NextDoor.Services.Identity.Services;
using NextDoor.Services.Identity.Services.Dto;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Controllers
{
    [Route("")]
    [ApiController]
    public class IdentityController : BaseController
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet("me")]
        [JwtAuth]
        public IActionResult Get() => Content($"Your id: '{UserId:N}'.");

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(SignUpDto signUp)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid");
            }

            //await _identityService.SignUpAsync(signUp.Email,
            //    signUp.Password, signUp.Role);

            return NoContent();
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInDto signIn)
            => Ok(await _identityService.SignInAsync(signIn.Email, signIn.Password));

        [HttpPut("change-pwd")]
        [JwtAuth]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto cp)
        {
            await _identityService.ChangePasswordAsync(cp.UserId,
                cp.CurrentPassword, cp.NewPassword);

            return NoContent();
        }
    }
}