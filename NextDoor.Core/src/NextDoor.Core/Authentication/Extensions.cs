using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NextDoor.Core.Common;
using NextDoor.Core.Types;
using System.Text;

namespace NextDoor.Core.Authentication
{
    public static class Extensions
    {
        public static void AddJwt(this IServiceCollection services)
        {
            IConfiguration configuration;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                configuration = serviceProvider.GetService<IConfiguration>();
                services.Configure<JwtOptions>(configuration.GetSection(ConfigOptions.jwtSectionName));
            }
            // Get jwt from app.config, bind to JwtOptions class model then inject as singleton
            var options = configuration.GetOptions<JwtOptions>(ConfigOptions.jwtSectionName);
            services.AddSingleton(options);
            // DI JwtHandler
            services.AddSingleton<IJwtHandler, JwtHandler>();
            // Add cancelling token service
            services.AddTransient<IAccessTokenService, AccessTokenService>();
            services.AddTransient<AccessTokenValidatorMiddleware>();

            // get JwtOptions for validation

            services.AddAuthentication()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey)),
                        ValidIssuer = options.ValidIssuer,
                        ValidAudience = options.ValidAudience,
                        ValidateAudience = options.ValidateAudience,
                        ValidateLifetime = options.ValidateLifetime
                    };
                });
        }

        /// <summary>
        /// Use AccessTokenValidatorMiddleware in "Configure"
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseAccessTokenValidator(this IApplicationBuilder app)
           => app.UseMiddleware<AccessTokenValidatorMiddleware>();
    }
}
