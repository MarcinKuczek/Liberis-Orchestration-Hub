using Liberis.OrchestrationHub.Application.Services;
using Liberis.Common.Test.Mocks;
using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Core.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Liberis.OrchestrationHub.Unit.Tests.Services
{
    public class ExampleServiceTests
    {
        private static IService Arrange(string content, HttpStatusCode statusCode)
        {
            var handler = new MockHttpMessageHandler(content, statusCode, "application/json");

            var httpClient = new HttpClient(handler);

            var opts = new ApiOptions
            {
                Uri = "https://postman-echo.com/get"
            };

            var mockOptions = Substitute.For<IOptions<ApiOptions>>();
            mockOptions.Value.Returns(opts);

            var mockFactory = Substitute.For<IHttpClientFactory>();
            mockFactory.CreateClient().Returns(httpClient);

            return new ExampleService(mockOptions, mockFactory);
        }

        [Fact]
        public async Task GetAsync_ReachableApi_ReturnsExampleResponse()
        {
            var sut = Arrange("{\"Message\": \"Ok\" }", HttpStatusCode.OK);

            var actual = await sut.GetAsync();

            Assert.True(actual.Successful);
            Assert.Equal(new JObject(new JProperty("Message", "Ok")).ToString(), actual.Message);
        }

        [Fact]
        public async Task GetAsync_UnreachableApi_ReturnsExampleResponse()
        {
            var sut = Arrange("", HttpStatusCode.NotFound);
            await Assert.ThrowsAsync<HttpRequestException>(() => sut.GetAsync());
        }
    }
}
