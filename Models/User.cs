namespace BookSwap.Models
{
    public class User
    {
        public int id { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public string? role { get; set; } // "Buyer", "Seller", "Admin"
        public string? SelectedRole { get; set; } // For role selection during login
    }
}
