using INTERNAL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace INTERNAL.Services.Implementations
{
    public class AgentBalanceScheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AgentBalanceScheduler> _logger;

        public AgentBalanceScheduler(IServiceProvider serviceProvider, ILogger<AgentBalanceScheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Agent Balance Scheduler is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var processor = scope.ServiceProvider.GetRequiredService<IAgentLowBalanceProcessingService>();
                        await processor.ProcessLowBalanceNotificationsAsync();
                        _logger.LogInformation("Agent balance check processed successfully at {time}", DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing agent balance notifications.");
                }

                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
