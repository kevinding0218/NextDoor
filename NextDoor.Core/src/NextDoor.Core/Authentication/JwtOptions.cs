using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Authentication
{
    public class JwtOptions
    {
        public string SecretKey { get; set; }
        public string ValidIssuer { get; set; }
        public int ExpiryMinutes { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateAudience { get; set; }
        public string ValidAudience { get; set; }
    }
}
