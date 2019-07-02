using System.Threading.Tasks;

namespace NextDoor.Core.Fabio
{
    public interface IFabioHttpClient
    {
        Task<T> GetAsync<T>(string requestUri);
    }
}
