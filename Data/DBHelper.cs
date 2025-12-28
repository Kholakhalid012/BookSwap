// using System.Data;
// using Microsoft.Data.SqlClient;

// namespace BookSwap.Data
// {
//     public static class DBHelper
//     {
//         private static readonly string connectionString =
//             // @"Server=localhost,1443; Database=BookSwap; User Id=sa; Password=StrongPass@123; TrustServerCertificate=true;";

//             @"Server=host.docker.internal,1443; Database=BookSwap; User Id=sa; Password=StrongPass@123; TrustServerCertificate=true;";

//         public static IDbConnection CreateConnection()
//         {
//             return new SqlConnection(connectionString);
//         }
//     }
// }

using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BookSwap.Data
{
    public class DBHelper(IConfiguration configuration)
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
