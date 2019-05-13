using Newtonsoft.Json;

namespace NextDoor.Core.Messages
{
    public class RejectedEvent : IRejectedEvent
    {
        // the interface only requires the property has a getter, the implementation doesn't have to be read-only.
        public string Reason { get; }
        // Use "readonly" when you want to set the property only once. In the constructor or variable initializer.
        // Use "private set" when you want setter can't be accessed from outside.
        public string Code { get; private set; }

        // To specify that a constructor should be used to create a class during deserialization
        [JsonConstructor]
        public RejectedEvent(string reason, string code)
        {
            this.Reason = reason;
            this.Code = code;
        }

        public static IRejectedEvent For(string name)
            => new RejectedEvent($"There was an error when executing: " + $"{name}", $"{name}_error");
    }
}