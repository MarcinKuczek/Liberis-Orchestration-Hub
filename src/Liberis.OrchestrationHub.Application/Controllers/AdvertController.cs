using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Messages.V1.Advert;
using Microsoft.AspNetCore.Mvc;

namespace Liberis.OrchestrationHub.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertController : HubController<GetAdvertRequest>
    {
        public AdvertController(IHubService<GetAdvertRequest> hubService) : base(hubService)
        {
        }
    }
}
