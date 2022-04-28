using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace BPX.DAL.Context
{
    public class DPContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DPContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("connStrDbBPX");
        }

        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);
    }
}
