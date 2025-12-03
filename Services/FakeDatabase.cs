using BookSwap.Models;

namespace BookSwap.Services
{
    public static class FakeDatabase
    {
        // TEMP USERS
        public static List<User> Users = new()
        {
            new User { id = 1, username = "admin", password = "admin123", role = "Admin" },
            new User { id = 2, username = "seller1", password = "123", role = "Seller" },
            new User { id = 3, username = "buyer1", password = "123", role = "Buyer" }
        };

        // TEMP BOOKS
        public static List<Book> Books = new()
        {
            new Book { Id = 1, Title = "C# Fundamentals", Author = "Mark", Category = "Programming", Price = 500, ImagePath = "/images/b1.jpg" },
            new Book { Id = 2, Title = "Data Structures", Author = "Sara", Category = "CS", Price = 300, ImagePath = "/images/b2.jpg" }
        };

        // TEMP ORDERS
        public static List<Order> Orders = new();
    }
}
