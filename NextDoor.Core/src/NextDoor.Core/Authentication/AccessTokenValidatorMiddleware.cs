using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace NextDoor.Core.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessTokenValidatorMiddleware : IMiddleware
    {
        private readonly IAccessTokenService _accessTokenService;
        private readonly ILogger<AccessTokenValidatorMiddleware> _logger;

        /// <summary>
        /// Inject IAccessTokenService to every request
        /// </summary>
        /// <param name="accessTokenService"></param>
        public AccessTokenValidatorMiddleware(IAccessTokenService accessTokenService,
            ILogger<AccessTokenValidatorMiddleware> logger)
        {
            _accessTokenService = accessTokenService;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var httpRequest = $"AccessTokenValidatorMiddleware Invoking: [{context.Request.Method}] {context.Request.Scheme}://{context.Request.Host}{context.Request.Path}";
            _logger.LogInformation(httpRequest);

            // Check from Redis if stores any deactivate token
            if (await _accessTokenService.IsCurrentActiveToken())
            {
                await next(context);

                return;
            }
            // If token currently is not active, meaning it's on our blanklist
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            _logger.LogError("AccessTokenValidatorMiddleware: Inactive Jwt Token");
        }
    }
}
