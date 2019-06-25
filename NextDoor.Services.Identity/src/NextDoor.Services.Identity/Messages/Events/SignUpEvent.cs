using Newtonsoft.Json;
using NextDoor.Core.Messages;

namespace NextDoor.Services.Identity.Messages.Events
{
    public class SignUpEvent : IEvent
    {
        public int Id { get; set; }
        public string Email { get; }
        public string Role { get; }

        [JsonConstructor]
        public SignUpEvent(int userId, string email, string role)
        {
            Id = userId;
            Email = email;
            Role = role;
        }
    }
}
