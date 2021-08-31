using Liberis.Common.Services;
using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Core.Models;
using Liberis.OrchestrationHub.Core.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Application.Services
{
    public class ExampleService : ExternalServiceBase, IExampleService
    {
        private readonly ApiOptions _options;

        public ExampleService(IOptions<ApiOptions> options, IHttpClientFactory httpClientFactory) : base(httpClientFactory.CreateClient()) 
            => _options = options.Value;

        public async Task<ExampleResponse> GetAsync()
        {
            var response = await GetAsync<JObject>(_options.Uri, null);
            var strResponse = response.HasValues ? response.ToString() : "";

            var output = new ExampleResponse
            {
                Successful = !string.IsNullOrEmpty(strResponse),
                Message = strResponse
            };

            return output;
        }
    }
}
