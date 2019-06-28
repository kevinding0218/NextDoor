using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NextDoor.Core.Handlers;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Repositories;
using NextDoor.Services.Identity.Infrastructure.Mongo;
using NextDoor.Services.Identity.Messages.Commands;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Handlers
{
    public class SignUpCommandHandler : ICommandHandler<SignUpCmd>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserMongoRepository _userMongoRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private bool _useNoSql = true;

        public SignUpCommandHandler(IUserRepository userRepository,
            IUserMongoRepository userMongoRepository,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userMongoRepository = userMongoRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _useNoSql = Convert.ToBoolean(_configuration["datasource:relational"]);
        }

        public async Task HandleAsync(SignUpCmd command, ICorrelationContext context)
        {
            var userDomain = (User)null;
            if (_useNoSql)
            {
                userDomain = await _userMongoRepository.GetAsync(command.Email);
            }
            else
            {
                userDomain = await _userRepository.GetAsync(command.Email);
            }
            if (userDomain != null)
            {
                throw new NextDoorException(IdentityExceptionCode.EmailInUse,
                    $"Email: '{command.Email}' is already in use.");
            }
            else
            {
                userDomain = new User(command.Email, command.Role, string.Empty);
                userDomain.SetPassword(command.Password, _passwordHasher);
                await _userRepository.AddAsync(userDomain);
                await _userMongoRepository.AddAsync(userDomain);
            }
        }
    }
}
