
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace BookSwap.Hubs;
public class StockHub : Hub
{
    public async Task UpdateStock(int bookId, int newStock)
    {
        await Clients.All.SendAsync("ReceiveStockUpdate", bookId, newStock);
    }
}

