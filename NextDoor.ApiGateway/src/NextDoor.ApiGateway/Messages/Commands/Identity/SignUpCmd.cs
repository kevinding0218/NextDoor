using Newtonsoft.Json;
using NextDoor.Core.Messages;
using System;

namespace NextDoor.ApiGateway.Messages.Commands.Identity
{
    /// <summary>
    /// Immutable
    /// Custom routing key: #.identity.sign_up_cmd
    /// Command that used to publish to RabbitMQ Exchange via routing key like #.identity.sign_up_cmd
    /// </summary>
    [ExchangeNamespace("identity")]
    public class SignUpCmd : ICommand
    {
        public Guid CommandId { get; }
        public string Email { get; }
        public string Password { get; }
        public string Role { get; }

        [JsonConstructor]
        public SignUpCmd(string email, string password)
        {
            Email = email;
            Password = password;
            Role = email.Contains("@nextdoor.com") ? "admin" : "client";
        }
    }
}
