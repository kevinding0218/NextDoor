using Newtonsoft.Json;
using NextDoor.Core.Messages;

namespace NextDoor.Services.Identity.Messages.Events
{
    // Immutable
    [ExchangeNamespace("signalr")]
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
