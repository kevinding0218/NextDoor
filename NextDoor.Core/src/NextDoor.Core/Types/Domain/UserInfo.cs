using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Types.Domain
{
    public class UserInfo : IUserInfo
    {
        public int? UID { get; set; }
        public string Role { get; set; }

        public UserInfo()
        {

        }

        public UserInfo(int? uID, string role)
        {
            UID = uID;
            Role = role;
        }
    }
}
