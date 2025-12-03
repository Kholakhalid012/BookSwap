using System.Collections.Generic;
using BookSwap.Models;

namespace BookSwap.Models.Interfaces
{
    public interface IUserRepository
    {
        User getByUsername(string username);
        void register(User user);
        User login(string username, string password);
        List<User> getAll();
    }
}
