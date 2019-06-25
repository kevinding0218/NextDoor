using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextDoor.Services.Identity.Services
{
    public interface IOptionalClaimsProvider
    {
        Task<IDictionary<string, string>> GetAsync(int userId);
    }
}
