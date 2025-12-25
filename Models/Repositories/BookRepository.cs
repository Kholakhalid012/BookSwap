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
        public Book GetById(int id)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM Books WHERE id = @id";
                return conn.QueryFirstOrDefault<Book>(sql, new { id });
            }
        }

        public IEnumerable<Book> GetAll()
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM Books"; // TODO: specify fields don't use '*', and also apply Limit and offset (pagination) (means if we are having more than 1000 books it will return all, we should use pagination)
                return conn.Query<Book>(sql).ToList();
            }
        }

        public void Add(Book book)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = @"INSERT INTO Books 
                               (title, author, price, sellerid, category, imagepath, stock) 
                               VALUES (@Title, @Author, @Price, @SellerId, @Category, @ImagePath, @Stock)";
                conn.Execute(sql, book);
            }
        }

        public void Update(Book book)
        {
            using (var conn = DBHelper.CreateConnection())
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
            using var conn = DBHelper.CreateConnection();

           
            var hasOrders = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Orders WHERE BookId = @id",
                new { id });

            if (hasOrders > 0)
                return false; 

            conn.Execute("DELETE FROM Books WHERE Id = @id", new { id });
            return true;
        }

        public List<Book> GetBooksBySeller(string sellerId)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM Books WHERE sellerid = @sellerId";
                return conn.Query<Book>(sql, new { sellerId }).ToList();
            }
        }

        public List<string> GetAllCategories()
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT DISTINCT category FROM Books WHERE category IS NOT NULL";
                return conn.Query<string>(sql).ToList();
            }
        }

        public void AddCategory(string categoryName)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "INSERT INTO Categories (name) VALUES (@categoryName)";
                conn.Execute(sql, new { categoryName });
            }
        }
    }
}
