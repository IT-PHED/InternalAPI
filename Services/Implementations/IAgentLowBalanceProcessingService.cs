using System.Threading.Tasks;

namespace INTERNAL.Services.Interfaces
{
    public interface IAgentLowBalanceProcessingService
    {
        Task ProcessLowBalanceNotificationsAsync();
    }
}
