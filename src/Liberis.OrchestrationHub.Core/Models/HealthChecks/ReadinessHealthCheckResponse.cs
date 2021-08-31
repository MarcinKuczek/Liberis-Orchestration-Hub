using System.Collections.Generic;

namespace Liberis.OrchestrationHub.Core.Models.HealthChecks
{
    public class ReadinessHealthCheckResponse : HealthCheckResponse
    {
        public IEnumerable<HealthCheck> HealthChecks { get; set; }
    }
}