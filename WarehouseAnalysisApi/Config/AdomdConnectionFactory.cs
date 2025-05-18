using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.Extensions.Configuration;

namespace WarehouseAnalysisApi.Config
{
    public class AdomdConnectionFactory
    {
        private readonly string _connectionString;

        public AdomdConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration["SSAS:ConnectionString"]
                                ?? throw new InvalidOperationException("SSAS connection string is missing.");
        }

        public AdomdConnection CreateConnection()
        {
            var connection = new AdomdConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}