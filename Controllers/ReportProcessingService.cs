using INTERNAL.Services.Interfaces;
using INTERNAL.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace INTERNAL.Services.Implementations
{
    public class ReportProcessingService : IReportProcessingService
    {
        private readonly ICapturedMeterService _reportService;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;

        public ReportProcessingService(ICapturedMeterService reportService, IMailService mailService, IConfiguration config)
        {
            _reportService = reportService;
            _mailService = mailService;
            _config = config;
        }

        public async Task ProcessAndSendReportAsync()
        {
            var email = _config["Mail:RecipientEmail"];
            var data = await _reportService.GetCapturedMeterReport();

            string fileName = $"{DateTime.Now:dd_MMM_yyyy}_capture_meter_report.xlsx";
            string body;
            byte[] excel = null;

            if (data == null || data.Count == 0)
            {
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "NoDataReport.html");
                body = await File.ReadAllTextAsync(templatePath);
            }
            else
            {
                excel = ExcelGenerator.Generate(data);
                var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "EmailReport.html");
                body = await File.ReadAllTextAsync(templatePath);
            }

            await _mailService.SendMailAsync(
                email,
                "Captured Meter Daily Report",
                body,
                excel,
                fileName);
        }
    }
}
