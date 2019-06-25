using Microsoft.AspNetCore.Identity;
using NextDoor.Core.Types.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Infrastructure.Domain
{
    public class User : IIdIdentifiable, IAuditableEntity
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

        protected User()
        {
        }

        public User(string email, string role, string passwordHash)
        {
            Email = email;
            Role = role;
            PasswordHash = passwordHash;
        }
    }
}
