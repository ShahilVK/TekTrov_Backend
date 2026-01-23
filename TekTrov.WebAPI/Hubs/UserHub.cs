using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace TekTrov.WebApi.Hubs
{
    [Authorize] // ✅ Required: JWT auth for SignalR
    public class UserHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (string.IsNullOrWhiteSpace(userId))
            {
                // 🚫 No user → block connection
                Context.Abort();
                return;
            }

            // ✅ Each user has their own group (userId)
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
