using System;

namespace NextDoor.Core.Types
{
    public interface IIdentifiable
    {
         Guid Id { get; }
    }
}