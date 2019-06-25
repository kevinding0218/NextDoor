using Newtonsoft.Json;

namespace NextDoor.Services.Identity.Services.Dto
{
    public class ChangePasswordDto
    {
        public int UserId { get; }
        public string CurrentPassword { get; }
        public string NewPassword { get; }

        [JsonConstructor]
        public ChangePasswordDto(int userId,
            string currentPassword, string newPassword)
        {
            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }
}
