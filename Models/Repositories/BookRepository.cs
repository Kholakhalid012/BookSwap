using Dapper;
using System.Collections.Generic;
using System.Linq;
using BookSwap.Models;
using BookSwap.Data;
using BookSwap.Models.Interfaces;

namespace BookSwap.Models.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly DBHelper _dbHelper;
        public BookRepository(DBHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public Book GetById(int id)
        {
            using (var conn = _dbHelper.CreateConnection())
            {
                string sql = "SELECT * FROM Books WHERE id = @id";
                return conn.QueryFirstOrDefault<Book>(sql, new { id });
            }
        }

       public IEnumerable<Book> GetAll()
        {
            using (var conn = _dbHelper.CreateConnection())
            {
                string sql = @"
                    SELECT Id, Title, Author, Category, Price, Stock, ImagePath, SellerId, IsDeleted
                    FROM Books
                    WHERE IsDeleted = 0
                    ORDER BY Id DESC
                ";

                return conn.Query<Book>(sql).ToList();
            }
        }

        public void Add(Book book)
        {
            using (var conn = _dbHelper.CreateConnection())
            {
                string sql = @"INSERT INTO Books 
                               (title, author, price, sellerid, category, imagepath, stock) 
                               VALUES (@Title, @Author, @Price, @SellerId, @Category, @ImagePath, @Stock)";
                conn.Execute(sql, book);
            }
        }

        public void Update(Book book)
        {
            using (var conn = _dbHelper.CreateConnection())
            {
                string sql = @"UPDATE Books 
                               SET title=@Title, author=@Author, price=@Price, sellerid=@SellerId, 
                                   category=@Category, imagepath=@ImagePath, stock=@Stock
                               WHERE id=@Id";
                conn.Execute(sql, book);
            }
        }

        public bool Delete(int id)
        {
            using var conn = _dbHelper.CreateConnection();

           
            var hasOrders = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Orders WHERE BookId = @id",
                new { id });

            if (hasOrders > 0)
                return false; 

            conn.Execute("DELETE FROM Books WHERE Id = @id", new { id });
            return true;
        }

        public bool SoftDelete(int id)
        {
            using var conn = _dbHelper.CreateConnection();

            // Soft delete: mark as deleted instead of removing
            var affected = conn.Execute(
                "UPDATE Books SET IsDeleted = 1 WHERE Id = @id",
                new { id });

            return affected > 0;
        }

       public List<Book> GetBooksBySeller(string sellerId)
        {
            using var conn = _dbHelper.CreateConnection();
            return conn.Query<Book>(
                "SELECT * FROM Books WHERE SellerId = @sellerId AND IsDeleted = 0",
                new { sellerId }
            ).ToList();
        }


        public List<string> GetAllCategories()
        {
            using (var conn = _dbHelper.CreateConnection())
            {
                string sql = "SELECT DISTINCT category FROM Books WHERE category IS NOT NULL";
                return conn.Query<string>(sql).ToList();
            }
        }

        public void AddCategory(string categoryName)
        {
            using (var conn = _dbHelper.CreateConnection())
            {
                string sql = "INSERT INTO Categories (name) VALUES (@categoryName)";
                conn.Execute(sql, new { categoryName });
            }
        }
    }
}
