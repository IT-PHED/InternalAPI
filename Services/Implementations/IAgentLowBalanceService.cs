using INTERNAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace INTERNAL.Services.Interfaces
{
    public interface IAgentLowBalanceService
    {
        Task<List<AgentLowBalanceNotification>> GetLowBalanceAgents();
    }
}
