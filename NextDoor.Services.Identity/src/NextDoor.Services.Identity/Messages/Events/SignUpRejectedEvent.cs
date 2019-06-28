using Newtonsoft.Json;
using NextDoor.Core.Messages;

namespace NextDoor.Services.Identity.Messages.Events
{
    // Immutable
    public class SignUpRejectedEvent : IRejectedEvent
    {
        public string Email { get; }
        public string Reason { get; }
        public string Code { get; }

        [JsonConstructor]
        public SignUpRejectedEvent(string email, string reason, string code)
        {
            Email = email;
            Reason = reason;
            Code = code;
        }
    }
}
