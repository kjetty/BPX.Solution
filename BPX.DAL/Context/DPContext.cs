using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
