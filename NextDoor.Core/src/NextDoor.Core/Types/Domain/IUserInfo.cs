using System;
using System.Collections.Generic;
using System.Text;

namespace NextDoor.Core.Types.Domain
{
    public interface IUserInfo
    {
        int? UID { get; set; }

        string Role { get; set; }
    }
}
