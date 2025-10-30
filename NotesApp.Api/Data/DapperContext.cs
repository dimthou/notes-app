using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace NotesApp.Api.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"📡 Connection String: {connectionString?.Substring(0, 50)}..."); // Debug log
            return new SqlConnection(connectionString);
        }
    }
}
