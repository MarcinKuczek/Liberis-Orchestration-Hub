using Liberis.OrchestrationHub.Core.Converters;
using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Messages.V1;
using MassTransit;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Application.Services
{
    public class HubService<T> : IHubService<T>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IAdapterNameProvider<T> _adapterProvider;

        public HubService(IPublishEndpoint publishEndpoint, IAdapterNameProvider<T> adapterProvider)
        {
            _publishEndpoint = publishEndpoint;
            _adapterProvider = adapterProvider;
        }

        public async Task SendRequestToAdapter(T request)
        {
            var routingKey = _adapterProvider.GetAdapterName(request);

            await _publishEndpoint.Publish<HubRequest<T>>(GetHubRequest(request, routingKey), x => x.SetRoutingKey(routingKey));
        }

        protected virtual HubRequest<T> GetHubRequest(T request, string routingKey)
        {
            return new HubRequest<T>
            {
                RequestId = GuidConverter.ConvertObject(request).ToString(),
                AdapterName = routingKey,
                Request = request
            };
        }
    }
}
