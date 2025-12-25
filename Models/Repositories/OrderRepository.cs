using Dapper;
using System.Collections.Generic;
using System.Linq;
using BookSwap.Models;
using BookSwap.Data;
using BookSwap.Models.Interfaces;

namespace BookSwap.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public bool placeOrder(Order order, out string message)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string getStockSql = "SELECT Stock FROM Books WHERE Id = @BookId";
                int stock = conn.QueryFirstOrDefault<int>(getStockSql, new { BookId = order.BookId });

                if (stock < order.Quantity)
                {
                    message = $"Not enough stock available. Current stock: {stock}";
                    return false; 
                }

                string insertSql = @"
                    INSERT INTO Orders (BookId, BuyerId, Quantity, TotalPrice, Status)
                    VALUES (@BookId, @BuyerId, @Quantity, @TotalPrice, @Status)";
                conn.Execute(insertSql, order);

                string updateStockSql = "UPDATE Books SET Stock = Stock - @Qty WHERE Id = @BookId";
                conn.Execute(updateStockSql, new { Qty = order.Quantity, BookId = order.BookId });

                message = "Order placed successfully!";
                return true;
            }
        }

    public List<Order> getOrdersByBuyer(string buyerId)
    {
        using (var conn = DBHelper.CreateConnection())
        {
            string sql = @"
             SELECT 
                o.id AS OrderId,
                o.bookid AS BookId,
                o.buyerid AS BuyerId,
                o.quantity AS Quantity,
                o.totalprice AS TotalPrice,
                o.status AS Status,

                b.id AS  BookId2,
                b.title AS Title,
                b.author AS Author,
                b.price AS Price,
                b.imagepath AS ImagePath,
                b.stock AS Stock,
                b.sellerid AS SellerId,
                b.sellername AS SellerName,
                b.sellercontact AS SellerContact
            FROM orders o
            INNER JOIN books b ON o.bookid = b.id
            WHERE o.buyerid = @buyerId";

            return conn.Query<Order, Book, Order>(
                sql,
                (order, book) =>
                {
                    order.Book = book;
                    return order;
                },
                new { buyerId },
                splitOn: "BookId2"
            ).ToList();
        }
    }


       public List<Order> getAllOrders()
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = @"
                   SELECT 
                        o.id AS OrderId,
                        o.bookid AS BookId,
                        o.buyerid AS BuyerId,
                        o.quantity AS Quantity,
                        o.totalprice AS TotalPrice,
                        o.status AS Status,

                        b.id AS BookId2,
                        b.title AS Title,
                        b.author AS Author,
                        b.price AS Price,
                        b.imagepath AS ImagePath,
                        b.stock AS Stock,
                        b.sellerid AS SellerId,
                        b.sellername AS SellerName,
                        b.sellercontact AS SellerContact
                    FROM orders o
                    INNER JOIN books b ON o.bookid = b.id";

                return conn.Query<Order, Book, Order>(
                    sql,
                    (order, book) =>
                    {
                        order.Book = book;
                        return order;
                    },
                    splitOn: "BookId2"
                ).ToList();
            }
        }


        public void updateStatus(int id, string status)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "UPDATE orders SET status = @status WHERE id = @id";
                conn.Execute(sql, new { id, status });
            }
        }
    }
}
