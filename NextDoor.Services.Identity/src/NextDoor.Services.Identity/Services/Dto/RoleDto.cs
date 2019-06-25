using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services.Dto
{
    public static class RoleDto
    {
        public const string User = "user";
        public const string Admin = "admin";

        public static bool IsValid(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }
            role = role.ToLowerInvariant();

            return role == User || role == Admin;
        }
    }
}
