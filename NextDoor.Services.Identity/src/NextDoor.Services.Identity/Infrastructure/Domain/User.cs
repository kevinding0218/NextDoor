using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Types;
using NextDoor.Core.Types.Domain;
using System;

namespace NextDoor.Services.Identity.Infrastructure.Domain
{
    public class User : IIdIdentifiable, IAuditableEntity, IGuidIdentifiable
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string Role { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime LastLogin { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public Guid Guid { get; set; }

        protected User()
        {
        }

        public User(string email)
        {
            Email = email;
            Role = email.Contains("@nextdoor.com") ? "admin" : "client";
        }

        public void SetPassword(string passwordTyped, IPasswordHasher<User> passwordHasher)
        {
            if (string.IsNullOrWhiteSpace(passwordTyped))
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidPassword,
                    "Password can not be empty.");
            }
            PasswordHash = passwordHasher.HashPassword(this, passwordTyped);
        }

        public bool ValidatePassword(string passwordTyped, IPasswordHasher<User> passwordHasher)
            => passwordHasher.VerifyHashedPassword(this, PasswordHash, passwordTyped) != PasswordVerificationResult.Failed;
    }
}
