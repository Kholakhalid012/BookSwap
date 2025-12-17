using System.ComponentModel.DataAnnotations.Schema;

namespace BookSwap.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("SellerId")]
        public ApplicationUser? Seller { get; set; }

        public string? ImagePath { get; set; }
        public string? SellerId { get; set; }
        public string? SellerName { get; set; }     
        public string? SellerContact { get; set; }  
        public int Stock { get; set; } = 1; 
    }
}
