using BookSwap.Models;
namespace BookSwap.ViewModels
{
    public class AdminReportViewModel
    {
        public List<Order?> RecentOrders { get; set; } = new();
        public Dictionary<string, string?> UserDict { get; set; } = new();
        public Dictionary<int, Book?> BookDict { get; set; } = new();
        public int TotalUsers { get; set; }
        public int TotalBooks { get; set; }
        public int TotalOrders { get; set; }
        public int BuyerCount { get; set; }
        public int SellerCount { get; set; }
        public string? TopBuyerId { get; set; }
        public string? TopBuyerName { get; set; }
        public string? TopSellerId { get; set; }
        public string? TopSellerName { get; set; }
    }
}