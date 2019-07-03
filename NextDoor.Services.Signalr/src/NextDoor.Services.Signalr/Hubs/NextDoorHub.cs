using Microsoft.AspNetCore.SignalR;
using NextDoor.Core.Authentication;
using NextDoor.Services.Signalr.Framework;
using System;
using System.Threading.Tasks;

namespace NextDoor.Services.Signalr.Hubs
{
    public class NextDoorHub : Hub
    {
        private readonly IJwtHandler _jwtHandler;

        public NextDoorHub(IJwtHandler jwtHandler)
        {
            _jwtHandler = jwtHandler;
        }

        public async Task InitializeAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                await DisconnectAsync();
            }
            try
            {
                var payload = _jwtHandler.GetTokenPayload(token);
                if (payload == null)
                {
                    await DisconnectAsync();

                    return;
                }
                var group = Convert.ToInt32(payload.Subject).ToUserGroup();
                await Groups.AddToGroupAsync(Context.ConnectionId, group);
                await ConnectAsync();
            }
            catch
            {
                await DisconnectAsync();
            }
        }

        private async Task ConnectAsync()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("connected");
        }

        private async Task DisconnectAsync()
        {
            await Clients.Client(Context.ConnectionId).SendAsync("disconnected");
        }
    }
}
