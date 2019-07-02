using Newtonsoft.Json;
using NextDoor.Core.Messages;

namespace NextDoor.Services.Operations.Messages.Identity.Events
{
    [ExchangeNamespace("identity")]
    public class SignUpRejectedEvent : IRejectedEvent
    {
        public string Reason { get; }
        public string Code { get; }

        [JsonConstructor]
        public SignUpRejectedEvent(string reason, string code)
        {
            Reason = reason;
            Code = code;
        }
    }
}
