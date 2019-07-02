using Consul;
using System.Threading.Tasks;

namespace NextDoor.Core.Consul
{
    public interface IConsulServicesRegistry
    {
        Task<AgentService> GetAsync(string name);
    }
}
