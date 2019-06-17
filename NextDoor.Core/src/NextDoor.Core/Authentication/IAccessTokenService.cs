using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NextDoor.Core.Authentication
{
    // https://piotrgankiewicz.com/2018/04/25/canceling-jwt-tokens-in-net-core/
    public interface IAccessTokenService
    {
        // Helper method - is current token for the current user actually active or not
        Task<bool> IsCurrentActiveToken();
        // Helper method - based on our Http context of the current user we'll check if his token is active 
        // and will deactive his token if he wishes so
        Task DeactivateCurrentAsync(string userId);

        // Determine whether our authorization or authentication token is active or not
        Task<bool> IsActiveAsync(string token);
        // Deactivate our token
        Task DeactivateAsync(string userId, string token);
    }
}
