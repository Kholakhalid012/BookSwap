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
                string sql = "SELECT * FROM books WHERE id = @id";
                return conn.QueryFirstOrDefault<Book>(sql, new { id });
            }
        }

        public IEnumerable<Book> GetAll()
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM books";
                return conn.Query<Book>(sql).ToList();
            }
        }

        public void Add(Book book)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = @"INSERT INTO books 
                               (title, author, price, sellerid, category, imagepath) 
                               VALUES (@Title, @Author, @Price, @SellerId, @Category, @ImagePath)";
                conn.Execute(sql, book);
            }
        }

        public void Update(Book book)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = @"UPDATE books 
                               SET title=@Title, author=@Author, price=@Price, sellerid=@SellerId, 
                                   category=@Category, imagepath=@ImagePath
                               WHERE id=@Id";
                conn.Execute(sql, book);
            }
        }

        public void Delete(int id)
        {
            using (var conn = DBHelper.CreateConnection())
            {
               string sql = "DELETE FROM Books WHERE Id = @id";
                conn.Execute(sql, new { id });
            }
        }

        public List<Book> GetBooksBySeller(string sellerId)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM books WHERE sellerid = @sellerId";
                return conn.Query<Book>(sql, new { sellerId }).ToList();
            }
        }

        public List<string> GetAllCategories()
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT DISTINCT category FROM books WHERE category IS NOT NULL";
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
