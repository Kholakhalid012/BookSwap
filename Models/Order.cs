
using Microsoft.CodeAnalysis;
namespace BookSwap.Models
{       
public class Order
    {
        public string? id { get; set; }
        public int bookid { get; set; }
        public string? buyerid { get; set; }
        public int quantity { get; set; }
        public double totalprice { get; set; }
        public string? status { get; set; } // Pending / Completed
    }
}


