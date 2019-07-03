using Newtonsoft.Json;
using NextDoor.Core.Messages;
using System;

namespace NextDoor.Services.Signalr.Messages.Events
{
    [ExchangeNamespace("signalr")]
    public class OperationPending : IEvent
    {
        public Guid Id { get; }
        public int UserId { get; }
        public string Name { get; }
        public string Resource { get; }

        [JsonConstructor]
        public OperationPending(Guid id,
            int userId, string name, string resource)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Resource = resource;
        }
    }
}
