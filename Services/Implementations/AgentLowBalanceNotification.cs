using System;

namespace INTERNAL.Models
{
    public class AgentLowBalanceNotification
    {
        public string Superagent_Id { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public decimal Closing_Balance { get; set; }
        public string CreatedBy { get; set; }
    }
}
