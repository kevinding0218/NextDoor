using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NextDoor.Core.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessTokenValidatorMiddleware : IMiddleware
    {
        private readonly IAccessTokenService _accessTokenService;

        /// <summary>
        /// Inject IAccessTokenService to every request
        /// </summary>
        /// <param name="accessTokenService"></param>
        public AccessTokenValidatorMiddleware(IAccessTokenService accessTokenService)
        {
            _accessTokenService = accessTokenService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            if (await _accessTokenService.IsCurrentActiveToken())
            {
                await next(context);

                return;
            }
            // If token currently is not active, meaning it's on our blanklist
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }
    }
}
