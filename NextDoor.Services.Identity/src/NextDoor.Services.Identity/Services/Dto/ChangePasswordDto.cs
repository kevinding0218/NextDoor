﻿using Newtonsoft.Json;
using NextDoor.Core.Types;

namespace NextDoor.Services.Identity.Messages.Commands
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
            if (newPassword.Length < 8)
            {
                throw new NextDoorException(IdentityExceptionCode.InvalidPassword,
                    $"Password length should be at least 8 characters.");
            }

            UserId = userId;
            CurrentPassword = currentPassword;
            NewPassword = newPassword;
        }
    }
}
