using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Liberis.OrchestrationHub.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        private readonly ILogger<ExampleController> _logger;
        private readonly IService _service;

        public ExampleController(ILogger<ExampleController> logger, IService service)
        {
            _logger = logger;
            _service = service;
        }

        /// <summary>
        /// Example endpoint
        /// </summary>
        /// <returns>Response from the example service</returns>
        /// <response code="200">Output from the example service</response>
        /// <response code="400">If the example service reports an error or calling the service failed</response>
        [HttpGet]
        [ProducesResponseType(typeof(ExampleResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExampleResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var output = await _service.GetAsync();
                return output.Successful ? (IActionResult) Ok(output) : BadRequest(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {method} in {controller}", nameof(GetAsync), nameof(ExampleController));
                return BadRequest();
            }
        }        
        
        /// <summary>
        /// Example endpoint that uses fluent validation on the request.
        /// </summary>
        /// <returns>Response from the example service</returns>
        /// <response code="200">Output from the example service</response>
        /// <response code="400">If the example service reports an error or calling the service failed</response>
        [HttpGet("query")]
        [ProducesResponseType(typeof(ExampleResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExampleResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetWithQueryAsync([FromQuery] ExampleRequest example)
        {
            try
            {
                var output = await _service.GetAsync();
                return output.Successful ? (IActionResult) Ok(output) : BadRequest(output);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during {method} in {controller}", nameof(GetAsync), nameof(ExampleController));
                return BadRequest();
            }
        }

        /// <summary>
        /// Example endpoint with authorization enabled
        /// </summary>
        /// <returns>Response from the example service</returns>
        /// <response code="200">Output from the example service</response>
        /// <response code="400">If the example service reports an error or calling the service failed</response>
        /// <response code="401">If unauthorized</response>
        [Authorize]
        [HttpGet("secure")]
        [ProducesResponseType(typeof(ExampleResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ExampleResponse), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ExampleResponse), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetSecureAsync()
        {
            return await GetAsync();
        }
    }
}