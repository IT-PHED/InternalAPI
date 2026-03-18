using INTERNAL.Models;
using INTERNAL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INTERNAL.Services.Implementations
{
    public class AgentLowBalanceProcessingService : IAgentLowBalanceProcessingService
    {
        private readonly IAgentLowBalanceService _dataService;
        private readonly IMailService _mailService;
        private readonly IConfiguration _config;
        private readonly ILogger<AgentLowBalanceProcessingService> _logger;

        public AgentLowBalanceProcessingService(IAgentLowBalanceService dataService, IMailService mailService, IConfiguration config, ILogger<AgentLowBalanceProcessingService> logger)
        {
            _dataService = dataService;
            _mailService = mailService;
            _config = config;
            _logger = logger;
        }

        public async Task ProcessLowBalanceNotificationsAsync()
        {
            var allLowBalanceAgents = await _dataService.GetLowBalanceAgents();

            if (allLowBalanceAgents == null || allLowBalanceAgents.Count == 0)
            {
                _logger.LogInformation("No agents with low balance detected. Skipping notifications.");
                return;
            }

            _logger.LogInformation("Processing {Count} low balance records for agents.", allLowBalanceAgents.Count);

            // Dynamically fetch all sub-sections under "Mail" that have a "To" configuration
            var agentConfigs = _config.GetSection("Mail").GetChildren()
                .Where(x => !string.IsNullOrEmpty(x["To"]));

            foreach (var section in agentConfigs)
            {
                string agentName = section.Key;
                var agents = allLowBalanceAgents
                    .Where(a => a.CreatedBy != null && a.CreatedBy.Trim().Equals(agentName.Trim(), StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (!agents.Any())
                {
                    _logger.LogDebug("Agent {AgentName} balance is healthy. No alert needed.", agentName);
                    continue;
                }

                var to = section["To"];
                var cc = section["Cc"];

                if (string.IsNullOrEmpty(to)) continue;

                var htmlBody = GenerateHtmlTable(agents, agentName);

                _logger.LogInformation("Sending low balance notification for agent: {AgentName}", agentName);

                await _mailService.SendMailAsync(
                    to,
                    $"Low Balance Notification - {agentName}",
                    htmlBody,
                    cc: cc);
            }
        }

        private string GenerateHtmlTable(List<AgentLowBalanceNotification> agents, string agentName)
        {
            var sb = new StringBuilder();
            var latest = agents.OrderByDescending(a => a.CreatedDateTime).First();

            sb.Append($@"
    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
      
      <!-- Header -->
      <div style='background-color: #d9534f; padding: 20px 24px;'>
        <h2 style='color: #ffffff; margin: 0; font-size: 18px;'>Low Balance Alert</h2>
      </div>

      <!-- Body -->
      <div style='padding: 24px; background-color: #ffffff;'>
        <p style='font-size: 15px; color: #333; margin: 0 0 16px;'>Dear <strong>{agentName}</strong>,</p>

        <p style='font-size: 14px; color: #555; margin: 0 0 20px; line-height: 1.6;'>
          We wish to notify you that your account balance is currently low. 
          As at <strong>{latest.CreatedDateTime:dd-MMM-yyyy HH:mm}</strong>, your closing balance stands at:
        </p>

        <!-- Balance Card -->
        <div style='background-color: #fff8f8; border-left: 4px solid #d9534f; padding: 16px 20px; border-radius: 4px; margin-bottom: 20px;'>
          <span style='font-size: 13px; color: #888; display: block; margin-bottom: 4px;'>Closing Balance</span>
          <span style='font-size: 28px; font-weight: bold; color: #d9534f;'>N{latest.Closing_Balance:N2}</span>
        </div>

        <p style='font-size: 14px; color: #555; line-height: 1.6; margin: 0 0 20px;'>
          To ensure uninterrupted and smooth transactions, kindly <strong>top up your account</strong> at your earliest convenience.
          Failure to fund your wallet may result in declined transactions.
        </p>");

            // Include full history table if multiple records
            if (agents.Count > 1)
            {
                sb.Append(@"
        <p style='font-size: 13px; color: #888; margin: 0 0 8px;'>Recent Balance History:</p>
        <table style='width:100%; border-collapse: collapse; font-size: 13px;'>
          <tr style='background-color: #f7f7f7; color: #555;'>
            <th style='padding: 8px; text-align: left; border-bottom: 1px solid #ddd;'>Date</th>
            <th style='padding: 8px; text-align: right; border-bottom: 1px solid #ddd;'>Closing Balance</th>
          </tr>");

                foreach (var agent in agents.OrderByDescending(a => a.CreatedDateTime))
                {
                    sb.Append($@"
          <tr>
            <td style='padding: 8px; border-bottom: 1px solid #f0f0f0; color: #444;'>{agent.CreatedDateTime:dd-MMM-yyyy HH:mm}</td>
            <td style='padding: 8px; border-bottom: 1px solid #f0f0f0; text-align: right; color: #d9534f; font-weight: 600;'>?{agent.Closing_Balance:N2}</td>
          </tr>");
                }

                sb.Append("</table>");
            }

            sb.Append($@"
      </div>

      <!-- Footer -->
      <div style='background-color: #f9f9f9; padding: 16px 24px; border-top: 1px solid #e0e0e0;'>
        <p style='font-size: 12px; color: #aaa; margin: 0;'>
          This is an automated notification. Please do not reply to this email.<br/>
          Generated by: <em>{latest.CreatedBy}</em>
        </p>
      </div>

    </div>");

            return sb.ToString();
        }
    }
}
