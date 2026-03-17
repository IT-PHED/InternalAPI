using INTERNAL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace INTERNAL.Services.Implementations
{
    public class ReportScheduler : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ReportScheduler> _logger;

        public ReportScheduler(IServiceProvider serviceProvider, ILogger<ReportScheduler> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                // Set target to 11:58 PM
                var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 23, 58, 0);
                //var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 16, 41, 0);

                if (now > nextRunTime)
                {
                    nextRunTime = nextRunTime.AddDays(1);
                }

                var delay = nextRunTime - now;
                _logger.LogInformation("Report Scheduler waiting until {nextRunTime}", nextRunTime);

                await Task.Delay(delay, stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<IReportProcessingService>();
                    await processor.ProcessAndSendReportAsync();
                    _logger.LogInformation("Daily report sent successfully at {time}", DateTime.Now);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing scheduled daily report.");
                }
            }
        }
    }
}
