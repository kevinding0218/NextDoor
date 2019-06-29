using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Authentication;
using NextDoor.Core.Common;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Repositories;
using NextDoor.Services.Identity.Infrastructure.Mongo;
using NextDoor.Services.Identity.Services.Dto;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserEFRepository _userEFRepository;
        private readonly IUserMongoRepository _userMongoRepository;
        private readonly IPasswordHasher<UserDto> _passwordHasher;
        private readonly IJwtHandler _jwtHandler;
        private readonly ITokenService _tokenService;
        private readonly IOptionalClaimsProvider _claimsProvider;
        private readonly IMapper _mapper;

        public IdentityService(
            IUserEFRepository userEFRepository,
            IUserMongoRepository userMongoRepository,
            IPasswordHasher<UserDto> passwordHasher,
            IJwtHandler jwtHandler,
            ITokenService tokenService,
            IOptionalClaimsProvider claimsProvider,
            IMapper mapper)
        {
            _userEFRepository = userEFRepository;
            _userMongoRepository = userMongoRepository;
            _passwordHasher = passwordHasher;
            _jwtHandler = jwtHandler;
            _tokenService = tokenService;
            _claimsProvider = claimsProvider;
            _mapper = mapper;
        }

        public async Task SignUpAsync(string email, string password)
        {
            var userDomain = (User)null;
            if (Shared.UseSql)
            {
                userDomain = await _userEFRepository.GetAsync(email);
            }
            else
            {
                userDomain = await _userMongoRepository.GetAsync(email);
            }

            if (userDomain != null)
            {
                throw new NextDoorException(IdentityExceptionCode.EmailInUse,
                    $"Email: '{email}' is already in use.");
            }

            var userDto = new UserDto(email, password);
            userDto.SetHashPassword(_passwordHasher);
            userDomain = _mapper.Map<UserDto, User>(userDto);

            if (Shared.UseSql)
            {
                await _userEFRepository.AddAsync(userDomain);
            }
            else
            {
                await _userMongoRepository.AddAsync(userDomain);
            }
        }

        public async Task<JsonWebToken> SignInAsync(string email, string password)
        {
            var userDomain = (User)null;
            if (Shared.UseSql)
            {
                userDomain = await _userEFRepository.GetAsync(email);
            }
            else
            {
                userDomain = await _userMongoRepository.GetAsync(email);
            }

            if (userDomain == null)
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidEmail,
                    $"Email: {email} is invalid.");
            }
            else
            {
                var userDto = _mapper.Map<User, UserDto>(userDomain);
                userDto.PasswordTyped = password;
                if (!userDto.ValidatePassword(_passwordHasher))
                {
                    throw new NextDoorException(IdentityExceptionCode.InvalidCredentials,
                        "Invalid credentials.");
                }

                // Revoke all existed tokens
                await _tokenService.RevokeAllExistedRefreshTokenAsync(userDto.Id);
                // Create New Refresh Token
                var refreshTokenDto = await _tokenService.CreateNewRefreshTokenAsync(userDto.Id);

                // Create New Jwt Access Token and bind with Refresh Token
                var jwt = await _tokenService.CreateNewJwtAccessTokenAsync(userDto.Id, userDto.Role, refreshTokenDto.Token);

                return jwt;
            }
        }

        public async Task UpdateLastLogin(User userDomain)
        {
            userDomain.Bind((u => u.LastLogin), DateTime.Now);

            if (Shared.UseSql)
            {
                await _userEFRepository.UpdateAsync(userDomain);
            }
            else
            {
                await _userMongoRepository.UpdateAsync(userDomain);
            }
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var userDomain = (User)null;
            if (Shared.UseSql)
            {
                userDomain = await _userEFRepository.GetAsync(userId);
            }
            else
            {
                userDomain = await _userMongoRepository.GetAsync(userId);
            }

            if (userDomain == null)
            {
                throw new NextDoorException(IdentityExceptionCode.UserNotFound,
                    $"User with id: '{userId}' was not found.");
            }

            var userDto = _mapper.Map<User, UserDto>(userDomain);
            userDto.PasswordTyped = currentPassword;
            if (!userDto.ValidatePassword(_passwordHasher))
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidCurrentPassword,
                    "Invalid current password.");
            }

            userDto.PasswordTyped = newPassword;
            userDto.SetHashPassword(_passwordHasher);
            userDomain = _mapper.Map<UserDto, User>(userDto);

            if (Shared.UseSql)
            {
                await _userEFRepository.UpdateAsync(userDomain);
            }
            else
            {
                await _userMongoRepository.UpdateAsync(userDomain);
            }
        }
    }
}
