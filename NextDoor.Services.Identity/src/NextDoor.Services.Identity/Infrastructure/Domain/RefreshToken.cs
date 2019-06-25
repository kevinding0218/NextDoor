using NextDoor.Core.Types.Domain;
using System;

namespace NextDoor.Services.Identity.Infrastructure.Domain
{
    public class RefreshToken : IIdIdentifiable
    {
        public int Id { get; private set; }
        public int Uid { get; private set; }
        public string Token { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        protected RefreshToken()
        {
        }
    }
}
