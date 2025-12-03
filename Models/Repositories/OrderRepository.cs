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
        public void placeOrder(Order order)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = @"INSERT INTO orders (bookid, buyerid, quantity, totalprice, status)
                               VALUES ( @bookid, @buyerid, @quantity, @totalprice, @status)";
                conn.Execute(sql, order);
            }
        }

        public List<Order> getOrdersByBuyer(string buyerId)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM orders WHERE buyerid = @buyerId";
                return conn.Query<Order>(sql, new { buyerId }).ToList();
            }
        }

        public List<Order> getAllOrders()
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM orders";
                return conn.Query<Order>(sql).ToList();
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
