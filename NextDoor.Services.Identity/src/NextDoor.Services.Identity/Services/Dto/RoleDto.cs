namespace NextDoor.Services.Identity.Services.Dto
{
    public static class RoleDto
    {
        public const string Client = "client";
        public const string Admin = "admin";

        public static bool IsValid(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }
            role = role.ToLowerInvariant();

            return role == Client || role == Admin;
        }
    }
}
