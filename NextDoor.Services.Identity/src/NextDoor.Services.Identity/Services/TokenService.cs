using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Authentication;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Repositories;
using NextDoor.Services.Identity.Infrastructure.Mongo;
using NextDoor.Services.Identity.Services.Dto;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRefreshTokenMongoRepository _refreshTokenMongoRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtHandler _jwtHandler;
        private readonly IPasswordHasher<UserDto> _passwordHasher;
        private readonly IOptionalClaimsProvider _optionalClaimsProvider;
        private readonly IMapper _mapper;

        public TokenService(
            IRefreshTokenRepository refreshTokenRepository,
            IRefreshTokenMongoRepository refreshTokenMongoRepository,
            IUserRepository userRepository,
            IJwtHandler jwtHandler,
            IPasswordHasher<UserDto> passwordHasher,
            IOptionalClaimsProvider optionalClaimsProvider,
            IMapper mapper)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _refreshTokenMongoRepository = refreshTokenMongoRepository;
            _userRepository = userRepository;
            _jwtHandler = jwtHandler;
            _passwordHasher = passwordHasher;
            _optionalClaimsProvider = optionalClaimsProvider;
            _mapper = mapper;
        }

        public async Task<RefreshTokenDto> CreateNewRefreshTokenAsync(int userId)
        {
            var userDomain = await _userRepository.GetAsync(userId);
            if (userDomain == null)
            {
                throw new NextDoorException(IdentityExceptionCode.UserNotFound,
                    $"User: '{userId}' was not found.");
            }

            // Get User Information to create hashed token for RefreshToken's Token
            var userDto = _mapper.Map<User, UserDto>(userDomain);
            var refreshTokenDto = new RefreshTokenDto(userDto, _passwordHasher);
            refreshTokenDto.Role = userDto.Role;

            // Insert into Database
            var refreshTokenDomain = _mapper.Map<RefreshTokenDto, RefreshToken>(refreshTokenDto);
            await _refreshTokenRepository.AddAsync(refreshTokenDomain);
            await _refreshTokenMongoRepository.AddAsync(refreshTokenDomain);

            return refreshTokenDto;
        }

        public async Task<JsonWebToken> CreateNewJwtAccessTokenAsync(int Uid, string Role, string refreshToken)
        {
            // Get Optional Claims as of Uid
            var optionalClaims = await _optionalClaimsProvider.GetAsync(Uid);

            // Create new jwt access token based on Uid, Role and Optional Claims
            var jwt = _jwtHandler.CreateToken(Uid.ToString("N"), Role, optionalClaims);

            // Assign RefreshToken's token with Jwt
            jwt.RefreshToken = refreshToken;

            return jwt;
        }

        public async Task<JsonWebToken> RefreshExistedJwtAccessTokenAsync(string refreshToken)
        {
            #region Validate if received token existed or revoked
            var refreshTokenDomain = await _refreshTokenRepository.GetAsync(refreshToken);
            if (refreshTokenDomain == null)
            {
                throw new NextDoorException(IdentityExceptionCode.RefreshTokenNotFound,
                    $"Refresh token {refreshToken} was not found.");
            }

            var refreshTokenDto = _mapper.Map<RefreshToken, RefreshTokenDto>(refreshTokenDomain);
            if (refreshTokenDto.Revoked)
            {
                throw new NextDoorException(IdentityExceptionCode.RefreshTokenAlreadyRevoked,
                    $"Refresh token: '{refreshToken}' was revoked.");
            }
            #endregion

            // Revoke current refreshToken
            await RevokeRefreshTokenAsync(refreshTokenDomain);

            // Create New Refresh Token
            refreshTokenDto = await CreateNewRefreshTokenAsync(refreshTokenDto.Uid);

            // Create New Jwt Access Token and bind with Refresh Token
            var jwt = await CreateNewJwtAccessTokenAsync(refreshTokenDto.Uid, refreshTokenDto.Role, refreshTokenDto.Token);

            return jwt;
        }

        private async Task RevokeRefreshTokenAsync(RefreshToken refreshTokenDomain)
        {
            await _refreshTokenRepository.RevokeAsync(refreshTokenDomain);
            await _refreshTokenMongoRepository.RevokeAsync(refreshTokenDomain);
        }

        public async Task RevokeRefreshTokenAsync(string token, int userId)
        {
            var refreshTokenDomain = await _refreshTokenRepository.GetAsync(token);
            if (refreshTokenDomain == null || refreshTokenDomain.Uid != userId)
            {
                throw new NextDoorException(IdentityExceptionCode.RefreshTokenNotFound,
                    "Refresh token was not found.");
            }

            var refreshTokenDto = _mapper.Map<RefreshToken, RefreshTokenDto>(refreshTokenDomain);
            refreshTokenDto.Revoke();

            await _refreshTokenRepository.RevokeAsync(refreshTokenDomain);
            await _refreshTokenMongoRepository.RevokeAsync(refreshTokenDomain);
        }

        public async Task RevokeAllExistedRefreshTokenAsync(int userId)
        {
            var existedTokens = await _refreshTokenRepository.GetListForActiveTokenAsync(userId);

            if (existedTokens != null)
            {
                foreach (var token in existedTokens)
                {
                    await RevokeRefreshTokenAsync(token.Token, userId);
                }
            }
        }
    }
}
