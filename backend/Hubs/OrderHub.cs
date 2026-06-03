using Microsoft.AspNetCore.SignalR;

namespace backend.Hubs;

public class OrderHub : Hub
{
    public async Task JoinUserGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
    }
}
