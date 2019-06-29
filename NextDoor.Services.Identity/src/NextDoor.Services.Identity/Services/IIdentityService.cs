using NextDoor.Core.Authentication;
using NextDoor.Services.Identity.Infrastructure.Domain;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services
{
    public interface IIdentityService
    {
        Task SignUpAsync(string email, string password);
        Task<JsonWebToken> SignInAsync(string email, string password);
        Task UpdateLastLogin(User userDomain);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
