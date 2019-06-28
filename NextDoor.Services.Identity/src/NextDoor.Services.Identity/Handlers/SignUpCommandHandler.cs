using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NextDoor.Core.Handlers;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Repositories;
using NextDoor.Services.Identity.Infrastructure.Mongo;
using NextDoor.Services.Identity.Messages.Commands;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Handlers
{
    public class SignUpCommandHandler : ICommandHandler<SignUpCmd>
    {
        private readonly IUserEFRepository _userEFRepository;
        private readonly IUserMongoRepository _userMongoRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public SignUpCommandHandler(IUserEFRepository userRepository,
            IUserMongoRepository userMongoRepository,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _userEFRepository = userRepository;
            _userMongoRepository = userMongoRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task HandleAsync(SignUpCmd command, ICorrelationContext context)
        {
            var userDomain = (User)null;
            if (Shared.UseSql)
            {
                userDomain = await _userEFRepository.GetAsync(command.Email);
            }
            else
            {
                userDomain = await _userMongoRepository.GetAsync(command.Email);
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
                if (Shared.UseSql)
                {
                    await _userEFRepository.AddAsync(userDomain);
                }
                else
                {
                    await _userMongoRepository.AddAsync(userDomain);
                }
            }
        }
    }
}
