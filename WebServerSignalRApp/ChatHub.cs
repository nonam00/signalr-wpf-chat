using Microsoft.AspNetCore.SignalR;

public record struct Color(byte R, byte G, byte B);

namespace WebServerSignalRApp
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, Color color)
        {
            await Clients.All.SendAsync("Receive", user, message, color);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Others.SendAsync("Notify", $"{Context.ConnectionId} logged into the chat");
            await Clients.Caller.SendAsync("Notify", "you logged into the chat");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            
            await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} leaves the chat");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
