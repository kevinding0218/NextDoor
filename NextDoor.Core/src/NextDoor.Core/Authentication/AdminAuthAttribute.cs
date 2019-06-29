namespace NextDoor.Core.Authentication
{
    public class AdminAuthAttribute : JwtAuthAttribute
    {
        public AdminAuthAttribute() : base("admin")
        {
        }
    }
}
