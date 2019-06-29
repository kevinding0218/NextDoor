using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace NextDoor.Core.Authentication
{
    // Custom Authorization Policy Attribute
    public class JwtAuthAttribute : AuthAttribute
    {
        public JwtAuthAttribute(string policy = "") : base(JwtBearerDefaults.AuthenticationScheme, policy)
        {
        }
    }
}
