using Liberis.OrchestrationHub.Application;
using Liberis.Common.Test.Fixtures;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Liberis.OrchestrationHub.Functional.Tests.Controllers
{
    public class ExampleControllerTests : IClassFixture<TestApiFixture<Startup>>
    {
        private const string InvalidToken =
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzUxMiJ9.eyJpc3MiOiJMaWJlcmlzIiwiYXVkIjoiV29ybGRwYXlVayIsImlhdCI6MTU5NzY1MzAyNCwiZXhwIjo5OTk5OTk5OTk5LCJzdWIiOiJNYXJrZXRwbGFjZSJ9.hlYlAaFYfIIpPWmrpenSuGVMio2Y6N2zipirLFHmK8dL6RclwrYrt49B_e-tPNzEjI3NDZFNSjRX-X_oDjzI6g";
        private readonly TestApiFixture<Startup> _client;

        public ExampleControllerTests(TestApiFixture<Startup> client)
            => _client = client;

        [Fact]
        public async Task ExampleRoute_Get_ReturnsOK()
        {
            var response = await _client.GetAsync("/api/example");

            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExampleRoute_Post_ReturnsMethodNotAllowed()
        {
            var response = await _client.PostAsync<string>("/api/example", null);

            Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task ExampleRoute_GetApiRoot_ReturnsSuccess()
        {
            var response = await _client.GetAsync("/");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ExampleRoute_GetNonExistentRoute_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/nope");

            Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ExampleRouteSecure_GetWithInvalidToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/example/secure", InvalidToken);

            Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }        
        
        [Fact]
        public async Task ExampleRouteSecure_GetWithNoToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/example/secure");

            Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
