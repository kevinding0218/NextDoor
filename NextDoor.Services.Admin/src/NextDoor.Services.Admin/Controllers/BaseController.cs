using Microsoft.AspNetCore.Mvc;
using NextDoor.Core.Dispatcher;
using System;

namespace NextDoor.Services.Admin.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        private readonly IDispatcher _dispatcher;

        public BaseController(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        #region Identity Info
        protected bool IsAdmin
            => User.IsInRole("admin");

        protected int UserId
             => string.IsNullOrWhiteSpace(User?.Identity?.Name) ?
                 0 : Convert.ToInt32(User.Identity.Name);
        #endregion
    }
}
