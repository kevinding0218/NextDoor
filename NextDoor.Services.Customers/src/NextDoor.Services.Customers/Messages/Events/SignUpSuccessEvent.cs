using Newtonsoft.Json;
using NextDoor.Core.Messages;

namespace NextDoor.Services.Customers.Messages.Events
{
    // Immutable
    [MessageNamespace("identity")]
    public class SignUpSuccessEvent : IEvent
    {
        public string Email { get; }

        [JsonConstructor]
        public SignUpSuccessEvent(string email)
        {
            Email = email;
        }
    }
}
