using NextDoor.Core.Types.Domain;
using System;

namespace NextDoor.Services.Admin.Infrastructure.Domain
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
    }
}
