using Newtonsoft.Json;
using NextDoor.Core.Messages;

namespace NextDoor.Services.Notifications.Messages.Events
{
    // Immutable
    [ExchangeNamespace("identity")]
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
