using Liberis.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Liberis.OrchestrationHub.Application.Controllers
{
     [Route("api/[controller]")]
     [ApiController]
     [ApiExplorerSettings(IgnoreApi = true)]
     public class ErrorController : ControllerBase
     {
         private readonly ILogger<ErrorController> _logger;
         
         public ErrorController(ILogger<ErrorController> logger)
         {
             _logger = logger;
         }
         
         [Route("")]
         public IActionResult Error()
         {
             var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
             if (context.Error is ForwardedStatusCodeException exception)
             {
                 return StatusCode((int) exception.StatusCode, exception.Content);
             }
             _logger.LogError(new EventId(1, "UnhandledException"), context.Error, context.Error.Message);
             return Problem();
         }
     }
}