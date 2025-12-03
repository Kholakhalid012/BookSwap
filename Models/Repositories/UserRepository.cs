using Dapper;
using System.Collections.Generic;
using System.Linq;
using BookSwap.Models;
using BookSwap.Data;
using BookSwap.Models.Interfaces;
using BCrypt.Net;

namespace BookSwap.Models.Repositories
{
    public class UserRepository : IUserRepository
    {
        // Get a user by username
        public User getByUsername(string username)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM users WHERE username = @username";
                return conn.QueryFirstOrDefault<User>(sql, new { username });
            }
        }

        // Register a new user with hashed password
        public void register(User user)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                // Hash the password before storing
                user.password = BCrypt.Net.BCrypt.HashPassword(user.password);

                string sql = @"INSERT INTO users (username, password, role)
                               VALUES (@username, @password, @role)";
                conn.Execute(sql, user);
            }
        }

        // Login user by verifying hashed password
        public User login(string username, string password)
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM users WHERE username = @username";
                var user = conn.QueryFirstOrDefault<User>(sql, new { username });

                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.password))
                {
                    return user;
                }

                return null; // Invalid login
            }
        }

        // Get all users
        public List<User> getAll()
        {
            using (var conn = DBHelper.CreateConnection())
            {
                string sql = "SELECT * FROM users";
                return conn.Query<User>(sql).ToList();
            }
        }
    }
}
