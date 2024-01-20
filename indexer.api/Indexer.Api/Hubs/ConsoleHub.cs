using Microsoft.AspNetCore.SignalR;


namespace Indexer.Api.Hubs;

public class ConsoleHub : Hub
{
    public async Task SendConsoleOutput(string output)
    {
        await Clients.All.SendAsync("ReceiveConsoleOutput", output);
    }
}