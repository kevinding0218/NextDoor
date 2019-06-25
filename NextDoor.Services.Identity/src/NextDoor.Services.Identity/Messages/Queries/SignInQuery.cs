using Newtonsoft.Json;
using NextDoor.Core.Authentication;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.Services.Identity.Messages.Queries
{
    public class SignInQuery : IQuery<JsonWebToken>
    {
        public string Email { get; }
        public string Password { get; }

        [JsonConstructor]
        public SignInQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
