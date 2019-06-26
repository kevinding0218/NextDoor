using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Authentication;
using NextDoor.Core.Handlers;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Repositories;
using NextDoor.Services.Identity.Messages.Queries;
using NextDoor.Services.Identity.Services;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Handlers
{
    public class SignInQueryHandler : IQueryHandler<SignInQuery, JsonWebToken>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;

        public SignInQueryHandler(
            IUserRepository userRepository,
            ITokenService tokenService,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }


        public async Task<JsonWebToken> HandleAsync(SignInQuery query)
        {
            var userDomain = await _userRepository.GetAsync(query.Email);

            if (userDomain == null)
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidEmail,
                    $"Email: {query.Email} is invalid.");
            }
            else
            {
                if (!userDomain.ValidatePassword(query.Password, _passwordHasher))
                {
                    throw new NextDoorException(IdentityExceptionCode.InvalidCredentials,
                        "Invalid credentials.");
                }

                await _tokenService.RevokeAllExistedRefreshTokenAsync(userDomain.Id);
                // Create New Refresh Token
                var refreshTokenDto = await _tokenService.CreateNewRefreshTokenAsync(userDomain.Id);

                // Create New Jwt Access Token and bind with Refresh Token
                var jwt = await _tokenService.CreateNewJwtAccessTokenAsync(userDomain.Id, userDomain.Role, refreshTokenDto.Token);

                return jwt;
            }
        }
    }
}
