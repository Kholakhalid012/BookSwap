// namespace BookSwap.Models
// {
//     // Book model for frontend-only
//   public class Book
//     {
//         public int Id { get; set; }
//         public string Title { get; set; } = "";
//         public string Author { get; set; } = "";
//         public string Category { get; set; } = "";
//         public decimal Price { get; set; }
//         public string ImagePath { get; set; } = "";
//         public string SellerName { get; set; } = "";
//         public string SellerContact { get; set; } = "";
//     }


//     // Order model for frontend-only
//   public class Order
//     {
//         public int id { get; set; }
//         public int bookid { get; set; }
//         public int buyerid { get; set; }
//         public int quantity { get; set; }
//         public double totalprice { get; set; }
//         public string? status { get; set; } // Pending / Completed
//     }

//     public class User
//     {
//         public int id { get; set; }
//         public string? username { get; set; }
//         public string? password { get; set; }
//         public string? role { get; set; } // "Buyer", "Seller", "Admin"
//         public string? SelectedRole { get; set; } // For role selection during login
//     }

// }
