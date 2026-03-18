using INTERNAL.Data;
using INTERNAL.Models;
using INTERNAL.Services.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace INTERNAL.Services.Implementations
{
    public class AgentLowBalanceService : IAgentLowBalanceService
    {
        private readonly OracleConnectionFactory _factory;

        public AgentLowBalanceService(OracleConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<AgentLowBalanceNotification>> GetLowBalanceAgents()
        {
            var result = new List<AgentLowBalanceNotification>();

            using var conn = _factory.GetConnection();
            using var cmd = new OracleCommand("SP_GET_LOW_BALANCE_AGENTS", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor)
                .Direction = ParameterDirection.Output;

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();

            var properties = typeof(AgentLowBalanceNotification).GetProperties();

            while (await reader.ReadAsync())
            {
                var item = new AgentLowBalanceNotification();
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
}
