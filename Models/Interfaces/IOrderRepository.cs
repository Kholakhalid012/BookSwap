using System.Collections.Generic;
using BookSwap.Models;

namespace BookSwap.Models.Interfaces
{
    public interface IOrderRepository
    {
        void placeOrder(Order order);
        List<Order> getOrdersByBuyer(string buyerId);
        List<Order> getAllOrders();
        void updateStatus(int id, string status);
    }
}
