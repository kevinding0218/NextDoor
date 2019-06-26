using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Types;
using System;

namespace NextDoor.Services.Identity.Services.Dto
{
    public class UserDto
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }
        public string PasswordTyped { get; set; }
        public string PasswordHash { get; private set; }
        public DateTime LastLogin { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string Guid { get; set; }

        protected UserDto() { }

        public UserDto(string email, string role, string passwordTyped)
        {
            Email = email;
            Role = role;
            PasswordTyped = passwordTyped;
        }

        public void SetHashPassword(IPasswordHasher<UserDto> passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(PasswordTyped))
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidPassword,
                    "Password can not be empty.");
            }
            PasswordHash = passwordHasher.HashPassword(this, PasswordTyped);
        }

        public bool ValidatePassword(IPasswordHasher<UserDto> passwordHasher)
            => passwordHasher.VerifyHashedPassword(this, this.PasswordHash, this.PasswordTyped) != PasswordVerificationResult.Failed;
    }


}
