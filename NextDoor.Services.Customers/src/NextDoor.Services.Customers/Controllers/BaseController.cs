using Microsoft.AspNetCore.Mvc;
using System;

namespace NextDoor.Services.Customers.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        #region Identity Info
        protected bool IsAdmin
            => User.IsInRole("admin");

        protected int UserId
             => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
                 0 : Convert.ToInt32(User.Identity.Name);
        #endregion
    }
}
