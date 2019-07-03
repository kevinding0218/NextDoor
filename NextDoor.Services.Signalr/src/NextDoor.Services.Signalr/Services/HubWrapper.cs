using Microsoft.AspNetCore.SignalR;
using NextDoor.Services.Signalr.Framework;
using NextDoor.Services.Signalr.Hubs;
using System.Threading.Tasks;

namespace NextDoor.Services.Signalr.Services
{
    public class HubWrapper : IHubWrapper
    {
        private readonly IHubContext<NextDoorHub> _hubContext;

        public HubWrapper(IHubContext<NextDoorHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishToUserAsync(int userId, string message, object data)
            => await _hubContext.Clients.Group(userId.ToUserGroup()).SendAsync(message, data);

        public async Task PublishToAllAsync(string message, object data)
            => await _hubContext.Clients.All.SendAsync(message, data);
    }
}
