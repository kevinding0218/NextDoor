namespace NextDoor.Services.Signalr.Framework
{
    public static class Extensions
    {
        public static string ToUserGroup(this int userId)
            => $"users:{userId}";
    }
}
