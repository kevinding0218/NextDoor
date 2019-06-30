using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.ApiGateway.Messages.Queries.Identity
{
    public class SignInQuery : IQuery<JsonWebToken>
    {
        public string Email { get; set; }
        public string Password { get; set; }

        [JsonConstructor]
        public SignInQuery(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
