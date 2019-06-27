using NextDoor.Core.Types.Domain;
using System;

namespace NextDoor.Services.Identity.Infrastructure.Domain
{
    public class RefreshToken : IIdIdentifiable, IGuidIdentifiable
    {
        public int Id { get; private set; }
        public int UID { get; private set; }
        public string Token { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public Guid Guid { get; set; }

        protected RefreshToken()
        {
        }
    }
}
