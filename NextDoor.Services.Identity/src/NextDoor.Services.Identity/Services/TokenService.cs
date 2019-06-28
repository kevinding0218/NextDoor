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
        private readonly IUserEFRepository _userEFRepository;
        private readonly IRefreshTokenEFRepository _refreshTokenEFRepository;

        private readonly IUserMongoRepository _userMongoRepository;
        private readonly IRefreshTokenMongoRepository _refreshTokenMongoRepository;

        private readonly IJwtHandler _jwtHandler;
        private readonly IPasswordHasher<UserDto> _passwordHasher;
        private readonly IOptionalClaimsProvider _optionalClaimsProvider;
        private readonly IMapper _mapper;

        public TokenService(
            IUserEFRepository userRepository,
            IUserMongoRepository userMongoRepository,
            IRefreshTokenEFRepository refreshTokenRepository,
            IRefreshTokenMongoRepository refreshTokenMongoRepository,
            IJwtHandler jwtHandler,
            IPasswordHasher<UserDto> passwordHasher,
            IOptionalClaimsProvider optionalClaimsProvider,
            IMapper mapper)
        {
            _userEFRepository = userRepository;
            _userMongoRepository = userMongoRepository;
            _refreshTokenEFRepository = refreshTokenRepository;
            _refreshTokenMongoRepository = refreshTokenMongoRepository;
            _jwtHandler = jwtHandler;
            _passwordHasher = passwordHasher;
            _optionalClaimsProvider = optionalClaimsProvider;
            _mapper = mapper;
        }

        public async Task<RefreshTokenDto> CreateNewRefreshTokenAsync(int Uid)
        {
            var userDomain = (User)null;
            if (Shared.UseSql)
            {
                userDomain = await _userEFRepository.GetAsync(Uid);
            }
            else
            {
                userDomain = await _userMongoRepository.GetAsync(Uid);
            }

            if (userDomain == null)
            {
                throw new NextDoorException(IdentityExceptionCode.UserNotFound,
                    $"User was not found.");
            }

            // Get User Information to create hashed token for RefreshToken's Token
            var userDto = _mapper.Map<User, UserDto>(userDomain);
            var refreshTokenDto = new RefreshTokenDto(userDto, _passwordHasher);
            refreshTokenDto.Role = userDto.Role;

            // Insert into Database
            var refreshTokenDomain = _mapper.Map<RefreshTokenDto, RefreshToken>(refreshTokenDto);
            if (Shared.UseSql)
            {
                await _refreshTokenEFRepository.AddAsync(refreshTokenDomain);
            }
            else
            {
                await _refreshTokenMongoRepository.AddAsync(refreshTokenDomain);
            }

            return refreshTokenDto;
        }

        public async Task<JsonWebToken> CreateNewJwtAccessTokenAsync(int Uid, string Role, string refreshToken)
        {
            // Get Optional Claims as of Uid
            var optionalClaims = await _optionalClaimsProvider.GetAsync(Uid);

            // Create new jwt access token based on Uid, Role and Optional Claims
            var jwt = _jwtHandler.CreateToken(Uid.ToString(), Role, optionalClaims);

            // Assign RefreshToken's token with Jwt
            jwt.RefreshToken = refreshToken;

            return jwt;
        }

        public async Task<JsonWebToken> RenewExistedJwtAccessTokenAsync(string refreshToken)
        {
            #region Validate if received token existed or revoked
            var refreshTokenDomain = (RefreshToken)null;
            var userDomain = (User)null;
            if (Shared.UseSql)
            {
                refreshTokenDomain = await _refreshTokenEFRepository.GetAsync(refreshToken);
                userDomain = await _userEFRepository.GetAsync(refreshTokenDomain.UID);
            }
            else
            {
                refreshTokenDomain = await _refreshTokenMongoRepository.GetAsync(refreshToken);
                userDomain = await _userMongoRepository.GetAsync(refreshTokenDomain.UID);
            }

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
            if (Shared.UseSql)
            {
                await _refreshTokenEFRepository.RevokeOneAsync(refreshTokenDomain);
            }
            else
            {
                await _refreshTokenMongoRepository.RevokeOneAsync(refreshTokenDomain);
            }
        }

        public async Task RevokeRefreshTokenAsync(string token, int userId)
        {
            var refreshTokenDomain = (RefreshToken)null;
            if (Shared.UseSql)
            {
                refreshTokenDomain = await _refreshTokenEFRepository.GetAsync(token);
            }
            else
            {
                refreshTokenDomain = await _refreshTokenMongoRepository.GetAsync(token);
            }

            if (refreshTokenDomain == null || refreshTokenDomain.UID != userId)
            {
                throw new NextDoorException(IdentityExceptionCode.RefreshTokenNotFound,
                    "Refresh token was not found.");
            }

            var refreshTokenDto = _mapper.Map<RefreshToken, RefreshTokenDto>(refreshTokenDomain);
            refreshTokenDto.Revoke();

            if (Shared.UseSql)
            {
                await _refreshTokenEFRepository.RevokeOneAsync(refreshTokenDomain);
            }
            else
            {
                await _refreshTokenMongoRepository.RevokeOneAsync(refreshTokenDomain);
            }
        }

        public async Task RevokeAllExistedRefreshTokenAsync(int userId)
        {
            var existedTokens = await _refreshTokenEFRepository.GetListForActiveTokenAsync(userId);

            if (existedTokens != null)
            {
                if (Shared.UseSql)
                {
                    await _refreshTokenEFRepository.RevokeListAsync(existedTokens);
                }
                else
                {
                    await _refreshTokenMongoRepository.RevokeListAsync(existedTokens);
                }
            }
        }
    }
}
