using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Dto
{
    public class UserDto
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public int Id { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }
        public string PasswordTyped { get; private set; }
        public string PasswordHash { get; private set; }

        protected UserDto() { }

        public UserDto(string email, string role, string passwordPlain)
        {
            if (!EmailRegex.IsMatch(email))
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidEmail,
                    $"Invalid email: '{email}'.");
            }
            if (!RoleDto.IsValid(role))
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidRole,
                    $"Invalid role: '{role}'.");
            }

            Email = email;
            Role = role;
            PasswordTyped = passwordPlain;
        }

        public void SetPassword(PasswordHasher<UserDto> passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(PasswordTyped))
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidPassword,
                    "Password can not be empty.");
            }
            PasswordHash = passwordHasher.HashPassword(this, PasswordTyped);
        }

        public bool ValidatePassword(IPasswordHasher<UserDto> passwordHasher)
            => passwordHasher.VerifyHashedPassword(this, PasswordHash, PasswordTyped) != PasswordVerificationResult.Failed;
    }


}
