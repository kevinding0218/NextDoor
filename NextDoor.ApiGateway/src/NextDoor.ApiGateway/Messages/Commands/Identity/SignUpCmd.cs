using Newtonsoft.Json;
using NextDoor.Core.Messages;
using System;

namespace NextDoor.ApiGateway.Messages.Commands.Identity
{
    // Custom routing key: #.identity.sign_up_cmd
    // ApiGateway has no clue that this command belongs to service A or B or C
    [MessageNamespace("identity")]
    public class SignUpCmd : ICommand
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Password { get; }
        public string Role { get; }

        [JsonConstructor]
        public SignUpCmd(string email, string password, string role)
        {
            Email = email;
            Password = password;
            Role = role ?? "user";
        }
    }
}
