﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Authentication;
using NextDoor.Services.Identity.Services;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Controllers
{
    [Route("")]
    [ApiController]
    [JwtAuth]
    public class TokenController : BaseController
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly ITokenService _tokenService;

        public TokenController(IAccessTokenService accessTokenService,
            ITokenService tokenService)
        {
            _accessTokenService = accessTokenService;
            _tokenService = tokenService;
        }

        [HttpPost("access-tokens/{refreshToken}/renew")]
        [AllowAnonymous]
        public async Task<IActionResult> RenewJwtAccessToken(string refreshToken)
            => Ok(await _tokenService.RenewExistedJwtAccessTokenAsync(refreshToken));

        [HttpPost("access-tokens/cancel")]
        public async Task<IActionResult> CancelJwtAccessToken()
        {
            await _accessTokenService.DeactivateCurrentAsync(UserId);

            return NoContent();
        }

        [HttpPost("refresh-tokens/{refreshToken}/revoke")]
        public async Task<IActionResult> RevokeRefreshToken(string refreshToken)
        {
            await _tokenService.RevokeRefreshTokenAsync(refreshToken, UserId);

            return NoContent();
        }
    }
}