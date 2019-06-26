using Microsoft.AspNetCore.Identity;
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
    public class SignUpQueryHandler : ICommandHandler<SignUpCmd>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserMongoRepository _userMongoRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public SignUpQueryHandler(IUserRepository userRepository,
            IUserMongoRepository userMongoRepository,
            IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _userMongoRepository = userMongoRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task HandleAsync(SignUpCmd command, ICorrelationContext context)
        {
            var userDomain = await _userRepository.GetAsync(command.Email);
            if (userDomain != null)
            {
                throw new NextDoorException(IdentityExceptionCode.EmailInUse,
                    $"Email: '{command.Email}' is already in use.");
            }

            userDomain = new User(command.Email, command.Role, string.Empty);
            userDomain.SetPassword(command.Password, _passwordHasher);
            await _userRepository.AddAsync(userDomain);
            // await _userMongoRepository.AddAsync(userDomain);
        }
    }
}
