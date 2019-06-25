using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Authentication;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Repositories;
using NextDoor.Services.Identity.Services.Dto;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<UserDto> _passwordHasher;
        private readonly IJwtHandler _jwtHandler;
        private readonly ITokenService _refreshTokenService;
        private readonly IOptionalClaimsProvider _claimsProvider;
        private readonly IMapper _mapper;

        public IdentityService(
            IUserRepository userRepository,
            IPasswordHasher<UserDto> passwordHasher,
            IJwtHandler jwtHandler,
            ITokenService refreshTokenService,
            IOptionalClaimsProvider claimsProvider,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtHandler = jwtHandler;
            _refreshTokenService = refreshTokenService;
            _claimsProvider = claimsProvider;
            _mapper = mapper;
        }

        public async Task SignUpAsync(string email, string password, string role = "user")
        {
            var userDomain = await _userRepository.GetAsync(email);
            if (userDomain != null)
            {
                throw new NextDoorException(IdentityExceptionCode.EmailInUse,
                    $"Email: '{email}' is already in use.");
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                role = RoleDto.User;
            }

            var userDto = new UserDto(email, role, password);
            userDto.SetHashPassword(_passwordHasher);
            userDomain = _mapper.Map<UserDto, User>(userDto);

            await _userRepository.AddAsync(userDomain);
        }

        public async Task<JsonWebToken> SignInAsync(string email, string password)
        {
            var userDomain = await _userRepository.GetAsync(email);

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

                // Create New Refresh Token
                var refreshTokenDto = await _refreshTokenService.CreateNewRefreshTokenAsync(userDto.Id);

                // Create New Jwt Access Token and bind with Refresh Token
                var jwt = await _refreshTokenService.CreateNewJwtAccessTokenAsync(userDto.Id, userDto.Role, refreshTokenDto.Token);

                return jwt;
            }
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var userDomain = await _userRepository.GetAsync(userId);
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

            await _userRepository.UpdateAsync(userDomain);
        }
    }
}
