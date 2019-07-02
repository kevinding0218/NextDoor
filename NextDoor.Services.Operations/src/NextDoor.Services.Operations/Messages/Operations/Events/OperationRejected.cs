using Newtonsoft.Json;
using NextDoor.Core.Messages;
using System;

namespace NextDoor.Services.Operations.Messages.Operations.Events
{
    public class OperationRejected : IEvent
    {
        public Guid Id { get; }
        public int UserId { get; }
        public string Name { get; }
        public string Resource { get; }
        public string Code { get; }
        public string Message { get; }

        [JsonConstructor]
        public OperationRejected(Guid id,
            int userId, string name, string resource,
            string code, string message)
        {
            Id = id;
            UserId = userId;
            Name = name;
            Resource = resource;
            Code = code;
            Message = message;
        }
    }
}
