using Newtonsoft.Json;
using NextDoor.Core.Messages;
using NextDoor.Core.Types;
using System;
using System.Text.RegularExpressions;

namespace NextDoor.Services.Identity.Messages.Commands
{
    /// <summary>
    /// Immutable
    /// Custom routing key: #.identity.sign_up_cmd
    /// Command that used to subscribe from RabbitMQ Exchange via routing key like #.identity.sign_up_cmd
    /// </summary>
    public class SignUpCmd : ICommand
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public Guid CommandId { get; }
        public string Email { get; }
        public string Password { get; }
        public string Role { get; }

        [JsonConstructor]
        public SignUpCmd(Guid commandId, string email, string password)
        {
            if (!EmailRegex.IsMatch(email))
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidEmail,
                    $"Invalid email: '{email}'.");
            }

            if (password.Length < 8)
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidPassword,
                    $"Password length should be at least 8 characters.");
            }

            CommandId = commandId;
            Email = email;
            Password = password;
            Role = email.Contains("@nextdoor.com") ? "admin" : "client";
        }
    }
}
