using System.Threading.Tasks;

namespace NextDoor.Core.Consul
{
    public interface IConsulHttpClient
    {
        Task<T> GetAsync<T>(string requestUri);
    }
}
