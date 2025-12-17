namespace BookSwap.Models
{
    public class Order
    {
        public int OrderId { get; set; }   // Primary Key

        public int BookId { get; set; }    // Foreign Key
        public Book? Book { get; set; }     // Navigation Property

        public string? BuyerId { get; set; } // Identity User Id

        public int Quantity { get; set; }

        public double TotalPrice { get; set; }

        public string? Status { get; set; } // Pending / Completed
    }
}
