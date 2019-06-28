﻿using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Handlers;
using NextDoor.Core.RabbitMq;
using NextDoor.Core.Types;
using NextDoor.Services.Identity.Infrastructure.Domain;
using NextDoor.Services.Identity.Infrastructure.EF.Repositories;
using NextDoor.Services.Identity.Infrastructure.Mongo;
using NextDoor.Services.Identity.Messages.Commands;
using NextDoor.Services.Identity.Messages.Events;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Handlers
{
    public class SignUpCommandHandler : ICommandHandler<SignUpCmd>
    {
        private readonly IUserEFRepository _userEFRepository;
        private readonly IUserMongoRepository _userMongoRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IBusPublisher _busPublisher;

        public SignUpCommandHandler(IUserEFRepository userRepository,
            IUserMongoRepository userMongoRepository,
            IPasswordHasher<User> passwordHasher,
            IBusPublisher busPublisher)
        {
            _userEFRepository = userRepository;
            _userMongoRepository = userMongoRepository;
            _passwordHasher = passwordHasher;
            _busPublisher = busPublisher;
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

                // Notify some other service that sign up successed
                await _busPublisher.PublishAsync(new SignUpSuccessEvent(userDomain.Email), context);
            }
        }
    }
}
