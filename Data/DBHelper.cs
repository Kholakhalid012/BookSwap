using System.Data;
using Microsoft.Data.SqlClient;

namespace BookSwap.Data
{
    public static class DBHelper
    {
        private static readonly string connectionString =
            @"Server=localhost,1443; Database=BookSwap; User Id=sa; Password=StrongPass@123; TrustServerCertificate=true;";

        public static IDbConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}

