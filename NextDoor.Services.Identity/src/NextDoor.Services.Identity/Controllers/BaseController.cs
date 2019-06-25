using Microsoft.AspNetCore.Mvc;
using System;

namespace NextDoor.Services.Identity.Controllers
{
    public class BaseController : ControllerBase
    {
        protected bool IsAdmin
            => User.IsInRole("admin");

        protected int UserId
            => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
                0 : Convert.ToInt32(User.Identity.Name);
    }
}