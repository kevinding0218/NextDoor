using System;

namespace NextDoor.Core.Types.Domain
{
    public interface IGuidIdentifiable : IEntity
    {
        Guid Guid { get; set; }
    }
}
