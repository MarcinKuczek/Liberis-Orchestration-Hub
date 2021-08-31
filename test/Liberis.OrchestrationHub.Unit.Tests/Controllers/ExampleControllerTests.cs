using Liberis.OrchestrationHub.Application.Controllers;
using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Core.Models;
using Liberis.OrchestrationHub.Tests.Common.Builders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace Liberis.OrchestrationHub.Unit.Tests.Controllers
{
    public class ExampleControllerTests
    {
        private static ExampleController Arrange(ExampleResponse response)
        {
            var service = Substitute.For<IService>();
            service.GetAsync().Returns(response);

            var logger = Substitute.For<ILogger<ExampleController>>();
            
            return new ExampleController(logger, service);
        }

        [Fact]
        public async Task GetAsync_SuccessfulApiResponse_ReturnsOkObjectResult()
        {
            var exampleResponse = new ExampleResponseBuilder()
                .WithMessage("Ok")
                .WithSuccess(true)
                .Build();

            var sut = Arrange(exampleResponse);

            var actual =  await sut.GetAsync();
            var okObjectResult = actual as OkObjectResult;

            Assert.NotNull(okObjectResult);
            Assert.Equal(okObjectResult.Value, exampleResponse);
        }        
        
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetAsync_UnsuccessfulApiResponse_ReturnsBadRequestObjectResult(string message)
        {
            var exampleResponse = new ExampleResponseBuilder()
                .WithMessage(message)
                .WithSuccess(false)
                .Build();

            var sut = Arrange(exampleResponse);

            var actual =  await sut.GetAsync();
            var badRequestObjectResult = actual as BadRequestObjectResult;

            Assert.NotNull(badRequestObjectResult);
            Assert.Equal(badRequestObjectResult.Value, exampleResponse);
        }

        [Fact]
        public async Task GetAsync_NullExampleResponse_BadRequestResult()
        {
            var sut = Arrange(null);

            var actual = await sut.GetAsync();
            var badRequestObjectResult = actual as BadRequestResult;

            Assert.NotNull(badRequestObjectResult);
        }
    }
}
