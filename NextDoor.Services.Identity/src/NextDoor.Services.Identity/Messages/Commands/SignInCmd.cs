using Newtonsoft.Json;
using NextDoor.Core.Messages;

namespace NextDoor.Services.Identity.Messages.Commands
{
    // Immutable
    public class SignInCmd : ICommand
    {
        public string Email { get; }
        public string Password { get; }

        [JsonConstructor]
        public SignInCmd(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
