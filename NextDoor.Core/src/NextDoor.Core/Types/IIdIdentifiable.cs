using System;

namespace NextDoor.Core.Types
{
    public interface IIdIdentifiable : IEntity
    {
        int Id { get; }
    }
}