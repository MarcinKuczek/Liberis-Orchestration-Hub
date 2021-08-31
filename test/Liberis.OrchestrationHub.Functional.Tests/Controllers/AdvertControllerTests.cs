using Liberis.Common.Test.Fixtures;
using Liberis.OrchestrationHub.Application;
using Liberis.OrchestrationHub.Core.Models;
using Liberis.OrchestrationHub.Functional.Tests.Bus;
using Liberis.OrchestrationHub.Messages.V1;
using Liberis.OrchestrationHub.Messages.V1.Advert;
using Liberis.Testing;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Liberis.OrchestrationHub.Functional.Tests.Controllers
{
    public class AdvertControllerTests : IClassFixture<TestApiFixture<Startup>>
    {
        private readonly TestApiFixture<Startup> _client;
        private TestBus _testBus;
        private TimedPromise<HubRequest<GetAdvertRequest>> _getAdvertRequestExpectation;

        public AdvertControllerTests(TestApiFixture<Startup> client)
        {
            _client = client;
            _testBus = new TestBus();
            
            var consumerConfiguration = new ConsumerConfiguration
            {
                ExchangeName = "Liberis.OrchestrationHub.Messages.V1:HubRequest--Liberis.OrchestrationHub.Messages.V1.Advert:GetAdvertRequest--",
                ExchangeType = "topic",
                RoutingKey = "orchestration.adapter",
            };

            _getAdvertRequestExpectation = _testBus.Expect<HubRequest<GetAdvertRequest>>(TimeSpan.FromMilliseconds(10000), consumerConfiguration);
        }

        [Fact(Skip = "Disabled due to missing RabbitMQ installation in pipeline")]
        public async Task AdvertRoute_Get_ReturnsOKAndPublishAdvert()
        {
            // Arrange
            await _testBus.StartAsync();

            var queryString = "PartnerCode=PartnerCodeTest&FirstName=FirstNameTest&Reference=ReferenceTest&Locale=LocaleTest&PxHeight=PxHeightTest&PxWidth=PxWidthTest&Source=SourceTest";

            // Act
            var response = await _client.GetAsync($"/api/advert?{queryString}");

            // Assert
            var actual = JsonConvert.DeserializeObject<HubResponse>(await response.Content.ReadAsStringAsync());
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Pending", actual.Status);
            Assert.Null(actual.Data);

            // Assert published Advert
            var advertRequest = _getAdvertRequestExpectation.WaitForResult().Request;
            Assert.Equal("PartnerCodeTest", advertRequest.PartnerCode);
            Assert.Equal("FirstNameTest", advertRequest.FirstName);
            Assert.Equal("ReferenceTest", advertRequest.Reference);
            Assert.Equal("LocaleTest", advertRequest.Locale);
            Assert.Equal("PxHeightTest", advertRequest.PxHeight);
            Assert.Equal("PxWidthTest", advertRequest.PxWidth);
            Assert.Equal("SourceTest", advertRequest.Source);
        }
    }
}
