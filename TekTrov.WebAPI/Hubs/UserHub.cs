using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TekTrov.WebApi.Hubs
{
    [Authorize] 
    public class UserHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
