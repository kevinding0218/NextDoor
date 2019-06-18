using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Types.Domain
{
    public interface IGuidIdentifiable : IEntity
    {
        Guid Guid { get; }
    }
}
