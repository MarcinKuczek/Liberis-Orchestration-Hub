using Liberis.OrchestrationHub.Core.Enums;
using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Application.Controllers
{
    [Route("api/[controller]")]
    public class HubController<T> : Controller
    {
        private readonly IHubService<T> _hubService;
        public HubController(IHubService<T> hubService)
        {
            _hubService = hubService;
        }

        /// <summary>
        /// Gets a resource from an external source
        /// </summary>
        /// <returns>Resource from an external source</returns>
        /// <response code="200">If request was sent successfully to RabbitMQ exchange</response>
        /// <response code="500">If there is an error in the process</response>
        [HttpPost]
        [ProducesResponseType(typeof(HubResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> PostAsync([FromBody] T request)
        {
            await _hubService.SendRequestToAdapter(request);

            var hubResponse = new HubResponse
            {
                Status = HubResponseStatus.Pending.ToString(),
                RequestedAt = DateTime.UtcNow,
                Data = null
            };

            return Ok(hubResponse);
        }
    }
}
