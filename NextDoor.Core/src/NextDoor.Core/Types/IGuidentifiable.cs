using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Types
{
    public interface IGuidentifiable : IEntity
    {
        Guid Guid { get; }
    }
}
