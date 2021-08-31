using Liberis.OrchestrationHub.Application.Models;
using Liberis.Common.Models;
using Liberis.Common.Providers;
using Liberis.OrchestrationHub.Core.Models;
using Liberis.OrchestrationHub.Core.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Application.Providers
{

    public class ExampleAuthProvider: AuthProviderBase, IExampleAuthProvider
    {
        private readonly ApiOptions _options;

        public ExampleAuthProvider(IOptions<ApiOptions> options, IDistributedCache cache)
            :base(cache)
        {
            _options = options.Value;
        }

        public override async Task<ExpirableAuthenticationHeaderValue> GetExpirableAuthAsync(Credentials credentials)
        {
            var tokenResponse = GetToken();

            return new ExpirableAuthenticationHeaderValue 
            { 
                Scheme = tokenResponse.TokenType, 
                Parameter = tokenResponse.AccessToken,
                ExpirationTime = TimeSpan.FromMinutes(_options.CacheLifeTimeMinutes)
            };
        
        }

        public override async Task<Credentials> GetCredentials()
        {
            return new ExampleCredentials
            {
                CliendId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            };
        }

        private ExampleTokenResponse GetToken()
        {
            var random = new Random();

            var token = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return new ExampleTokenResponse
            {
                TokenType = "Bearer",
                AccessToken = token
            };
        }
    }
}
