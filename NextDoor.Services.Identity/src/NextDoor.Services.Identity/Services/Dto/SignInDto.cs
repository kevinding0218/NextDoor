using Newtonsoft.Json;

namespace NextDoor.Services.Identity.Services.Dto
{
    public class SignInDto
    {
        public string Email { get; }
        public string Password { get; }

        [JsonConstructor]
        public SignInDto(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
