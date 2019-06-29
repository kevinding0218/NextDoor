using Microsoft.IdentityModel.JsonWebTokens;
using NextDoor.Core.Types.Pagination;

namespace NextDoor.ApiGateway.Messages.Queries.Identity
{
    public class SignInQuery : IQuery<JsonWebToken>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
