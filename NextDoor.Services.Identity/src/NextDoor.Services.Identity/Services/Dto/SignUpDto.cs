using Newtonsoft.Json;
using NextDoor.Core.Types;
using System.Text.RegularExpressions;

namespace NextDoor.Services.Identity.Services.Dto
{
    public class SignUpDto
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Email { get; }
        public string Password { get; }
        public string Role { get; } = "user";

        [JsonConstructor]
        public SignUpDto(string email, string password, string role)
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

            Email = email;
            Password = password;
            Role = role;
        }
    }
}
