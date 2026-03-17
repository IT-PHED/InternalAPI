using Oracle.ManagedDataAccess.Client;

namespace INTERNAL.Data;

public class OracleConnectionFactory
{
    private readonly IConfiguration _config;

    public OracleConnectionFactory(IConfiguration config)
    {
        _config = config;
    }

    public OracleConnection GetConnection()
    {
        return new OracleConnection(
            _config.GetConnectionString("Database"));
    }
}