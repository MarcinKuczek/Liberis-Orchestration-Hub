using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Messages.V1.Advert;

namespace Liberis.OrchestrationHub.Application.Providers
{
    public class AdvertAdapterNameProvider: IAdapterNameProvider<GetAdvertRequest>
    {
        public string GetAdapterName(GetAdvertRequest request)
        {
            // TBD: SHOULD BE BASED ON REQUEST
            return "orchestration.adapter";
        }
    }
}
