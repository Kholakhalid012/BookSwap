// using System.Collections.Generic;
// using BookSwap.Models;

// public static class DummyData
// {
//     public static List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>
//     {
//         new ApplicationUser { Id = "1", UserName = "admin", PasswordHash = "admin123", Role = "Admin", Email = "admin@example.com" },
//         new ApplicationUser { Id = "2", UserName = "seller1", PasswordHash = "seller123", Role = "Seller", Email = "seller1@example.com" },
//         new ApplicationUser { Id = "3", UserName = "buyer1", PasswordHash = "buyer123", Role = "Buyer", Email = "buyer1@example.com" }
//     };

//     public static List<Book> Books { get; set; } = new List<Book>
//     {
//         new Book { Id = 1, Title = "The Book of Life", Author = "Dr. Rajan Pandey", Category = "Life", Price = 3500, ImagePath = "/images/Book1.jpg", SellerId = "2" },
//         new Book { Id = 2, Title = "The Psychology of Money", Author = "Morgan Housel", Category = "Finance", Price = 4200, ImagePath = "/images/Book2.webp", SellerId = "2" },
//         new Book { Id = 3, Title = "Two Trees Make a Forest", Author = "Jessica J Lee", Category = "Nature", Price = 3800, ImagePath = "/images/Book3.gif", SellerId = "2" },
//         new Book { Id = 4, Title = "String Theory", Author = "Barton Zwiebach", Category = "Physics", Price = 2500, ImagePath = "/images/Book4.webp", SellerId = "2" },
//         new Book { Id = 5, Title = "Introduction to Computer Science", Author = "Gilbert Brands", Category = "Computer Science", Price = 4800, ImagePath = "/images/Book5.jpg", SellerId = "2" }
//     };

//     public static List<Order> Orders { get; set; } = new List<Order>
//     {
//         new Order { id = "1", bookid = Books[0].Id, buyerid = "3", quantity = 1, totalprice = (double)Books[0].Price, status = "Pending" },
//         new Order { id = "2", bookid = Books[2].Id, buyerid = "3", quantity = 2, totalprice = (double)Books[2].Price * 2, status = "Completed" }
//     };
// }
