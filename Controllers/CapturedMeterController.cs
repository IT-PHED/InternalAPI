using System;
using INTERNAL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace INTERNAL.Controllers
{
    [ApiController]
    [Route("api/CapturedMeterReport")]
    public class ReportController : ControllerBase
    {
        private readonly IReportProcessingService _processingService;

        public ReportController(IReportProcessingService processingService)
        {
            _processingService = processingService;
        }

        [HttpPost("DailySend")]
        public async Task<IActionResult> SendReport()
        {
            await _processingService.ProcessAndSendReportAsync();

            return Ok("Report sent");
        }
    }
}
