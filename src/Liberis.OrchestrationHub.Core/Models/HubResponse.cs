using System;

namespace Liberis.OrchestrationHub.Core.Models
{
    public class HubResponse
    {
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public object Data { get; set; }
    }
}
