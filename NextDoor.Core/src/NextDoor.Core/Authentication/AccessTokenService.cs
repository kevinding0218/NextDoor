using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextDoor.Core.Authentication
{
    /// <summary>
    /// what can we do in terms of canceling the active tokens? We have a few options:
    /// 1. Remove token on the client side (e.g. local storage) – will do the trick, but doesn’t really cancel the token.
    /// 2. Keep the token lifetime relatively short (5 minutes or so) – most likely we should do it anyway.
    /// 3. Create a blacklist of tokens that were deactivated – this is what we are going to focus on.
    /// </summary>
    public class AccessTokenService : IAccessTokenService
    {
        private readonly IDistributedCache _cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<JwtOptions> _jwtOptions;

        /// <summary>
        /// use the Redis to store the deactivated tokens on an extremely fast caching server. 
        /// Whether you host just a single instance of your application or multiple ones, 
        /// it’s the best idea to use Redis
        /// otherwise, when server goes down, 
        /// you will lose all of the deactivated tokens blacklist being kept in a default server cache 
        /// (not to mention the different data if each server would keep its own cache)
        /// </summary>
        /// <param name="cache">Using redis</param>
        /// <param name="httpContextAccessor">be able to access our current HTTP context for the particular user</param>
        /// <param name="jwtOptions"></param>
        public AccessTokenService(IDistributedCache cache,
            IHttpContextAccessor httpContextAccessor,
            IOptions<JwtOptions> jwtOptions)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            _jwtOptions = jwtOptions;
        }

        public async Task<bool> IsCurrentActiveToken()
            => await IsActiveAsync(GetCurrentTokenAsync());

        public async Task DeactivateCurrentAsync(string userId)
            => await DeactivateAsync(userId, GetCurrentTokenAsync());

        /// <summary>
        /// if we cannot get a compared string from cache meaning our token is still active
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> IsActiveAsync(string token)
            => string.IsNullOrWhiteSpace(await _cache.GetStringAsync(GetKey(token)));

        /// <summary>
        /// store our token in redis cache which would be a black list
        /// basic idea is to keep track of deactivated tokens only 
        /// and remove them from a cache when not needed anymore 
        /// (meaning when the expiry time passed) – they will be no longer valid anyway.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DeactivateAsync(string userId, string token)
        {
            // after certain time period, remove from our redis cache
            await _cache.SetStringAsync(GetKey(token),
                    "deactivated", new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromMinutes(_jwtOptions.Value.ExpiryMinutes)
                    });
        }

        /// <summary>
        /// Get current jwt token from HTTP context
        /// </summary>
        /// <returns></returns>
        private string GetCurrentTokenAsync()
        {
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(' ').Last();
        }

        /// <summary>
        /// getting our cache key
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static string GetKey(string token)
            => $"tokens:{token}";
    }
}
