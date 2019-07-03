using System.Threading.Tasks;

namespace NextDoor.Services.Signalr.Services
{
    public interface IHubWrapper
    {
        Task PublishToUserAsync(int userId, string message, object data);
        Task PublishToAllAsync(string message, object data);
    }
}
