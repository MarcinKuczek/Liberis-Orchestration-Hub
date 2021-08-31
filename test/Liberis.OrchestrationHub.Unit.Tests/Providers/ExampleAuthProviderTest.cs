using Liberis.OrchestrationHub.Application.Providers;
using Liberis.OrchestrationHub.Core.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Liberis.OrchestrationHub.Unit.Tests.Providers
{
    public class ExampleAuthProviderTest
    {
        private static ExampleAuthProvider Arrange(double cacheLifeTime)
        {
            var services = new ServiceCollection();
            services.AddDistributedMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            var memoryCache = serviceProvider.GetService<IDistributedCache>();


            var opts = new ApiOptions
            {
                Uri = "https://postman-echo.com/get",
                ClientId = "CliendId1",
                ClientSecret = "ClientSecret1",
                CacheLifeTimeMinutes = cacheLifeTime
            };

            var mockOptions = Substitute.For<IOptions<ApiOptions>>();
            mockOptions.Value.Returns(opts);


            return new ExampleAuthProvider(mockOptions, memoryCache);
        }

        [Fact]
        public async Task GetAuthAsync_SameTokenBeforeCacheExpires()
        {
            // 0.1 minutes = 6 seconds
            var cacheLifeTime = 0.1; 
            var sut = Arrange(cacheLifeTime);

            var AuthenticationAtemp1 = await sut.GetAuthAsync();

            var AuthenticationAtemp2task = Task.Run(() =>
            {
                return sut.GetAuthAsync();
            });

            bool isCompletedSuccessfully = AuthenticationAtemp2task.Wait(TimeSpan.FromMilliseconds(4000));

            if (isCompletedSuccessfully)
            {
                var AuthenticationAtemp2 = AuthenticationAtemp2task.Result;
                Assert.Equal(AuthenticationAtemp1.Parameter, AuthenticationAtemp2.Parameter);
            }
            else
            {
                throw new TimeoutException("The function has taken longer than the maximum time allowed.");
            }
        }


        [Fact]
        public async Task GetAuthAsync_NewTokenAfterCacheExpiration()
        {
            // 0.05 minutes = 3 seconds
            var cacheLifeTime = 0.05; 
            var sut = Arrange(cacheLifeTime);

            var AuthenticationAtemp1 = await sut.GetAuthAsync();
            System.Threading.Thread.Sleep(5000);
            var AuthenticationAtemp2 = await sut.GetAuthAsync();

            Assert.NotEqual(AuthenticationAtemp1.Parameter, AuthenticationAtemp2.Parameter);
        }
    }
}
