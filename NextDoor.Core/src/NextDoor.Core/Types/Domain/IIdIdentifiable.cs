using System;

namespace NextDoor.Core.Types.Domain
{
    public interface IIdIdentifiable : IEntity
    {
        int Id { get; }
    }
}