using INTERNAL.Data;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using INTERNAL.Models;
using INTERNAL.Services.Interfaces;


public class CapturedMeterService : ICapturedMeterService
{
    private readonly OracleConnectionFactory _factory;

    public CapturedMeterService(OracleConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<List<CapturedMeters>> GetCapturedMeterReport()
    {
        var result = new List<CapturedMeters>();

        using var conn = _factory.GetConnection();
        using var cmd = new OracleCommand("SPN_GET_TODAY_CAPTURED_METERS", conn);

        cmd.CommandType = CommandType.StoredProcedure;

        cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor)
            .Direction = ParameterDirection.Output;

        await conn.OpenAsync();

        using var reader = await cmd.ExecuteReaderAsync();

        var properties = typeof(CapturedMeters).GetProperties();

        while (await reader.ReadAsync())
        {
            var item = new CapturedMeters();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var colName = reader.GetName(i);
                var prop = properties.FirstOrDefault(p => p.Name.Equals(colName, StringComparison.OrdinalIgnoreCase));

                if (prop != null && reader[i] != DBNull.Value)
                {
                    prop.SetValue(item, Convert.ChangeType(reader[i], prop.PropertyType));
                }
            }
            result.Add(item);
        }

        return result;
    }
}