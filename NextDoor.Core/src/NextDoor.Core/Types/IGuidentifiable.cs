using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Types
{
    public interface IGuidIdentifiable : IEntity
    {
        Guid Guid { get; }
    }
}
