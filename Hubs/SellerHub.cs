using Microsoft.AspNetCore.SignalR;

public class SellerHub : Hub
{
    public async Task UpdateBookCount(int totalBooks)
    {
        await Clients.All.SendAsync("ReceiveBookCountUpdate", totalBooks);
    }
}
