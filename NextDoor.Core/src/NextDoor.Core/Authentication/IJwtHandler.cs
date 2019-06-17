using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Authentication
{
    public interface IJwtHandler
    {
        JsonWebToken CreateToken(string userId, string role = null, IDictionary<string, string> optionalClaims = null);
        JsonWebTokenPayload GetTokenPayload(string accessToken);
    }
}
