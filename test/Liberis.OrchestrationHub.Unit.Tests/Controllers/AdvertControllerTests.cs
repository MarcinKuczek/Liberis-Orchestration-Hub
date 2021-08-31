using Liberis.OrchestrationHub.Application.Controllers;
using Liberis.OrchestrationHub.Core.Enums;
using Liberis.OrchestrationHub.Core.Interfaces;
using Liberis.OrchestrationHub.Core.Models;
using Liberis.OrchestrationHub.Messages.V1.Advert;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Liberis.OrchestrationHub.Unit.Tests.Controllers
{
    public class AdvertControllerTests
    {
        private readonly IHubService<GetAdvertRequest> _mockHubService;

        public AdvertControllerTests()
        {
            _mockHubService = Substitute.For<IHubService<GetAdvertRequest>>();
        }

        [Fact]
        public async Task GetAsync_SuccessfulMessageBrokerResponse_ReturnsOkObjectResult()
        {
            // Arrange
            var advertRequest = new GetAdvertRequest
            {
                PartnerCode = "PartnerCode",
                FirstName = "FirstName",
                Reference = "Reference",
                Locale = "Locale",
                PxHeight = "PxHeight",
                PxWidth = "PxWidth",
                Source = "Source"
            };

            var expectedResponse = new HubResponse
            {
                Status = HubResponseStatus.Pending.ToString(),
                RequestedAt = DateTime.UtcNow,
                Data = null
            };

            _mockHubService.SendRequestToAdapter(advertRequest).Returns(Task.CompletedTask);
            var sut = new AdvertController(_mockHubService);

            // Act            
            var actual = await sut.PostAsync(advertRequest);

            // Assert
            await _mockHubService.Received().SendRequestToAdapter(Arg.Any<GetAdvertRequest>());
            var okObjectResult = actual as OkObjectResult;
            var actualResponse = okObjectResult.Value as HubResponse;
            Assert.NotNull(okObjectResult);
            Assert.Equal(expectedResponse.Status, actualResponse.Status);
            Assert.Equal(expectedResponse.Data, actualResponse.Data);
        }

        [Fact]
        public async Task GetAsync_FailedMessageBrokerResponse_ThrowsException()
        {
            // Arrange
            var advertRequest = new GetAdvertRequest
            {
                PartnerCode = "PartnerCode",
                FirstName = "FirstName",
                Reference = "Reference",
                Locale = "Locale",
                PxHeight = "PxHeight",
                PxWidth = "PxWidth",
                Source = "Source"
            };
            _mockHubService.When(x => x.SendRequestToAdapter(advertRequest)).Do(x => { throw new Exception(); }); ;
            var sut = new AdvertController(_mockHubService);

            // Act-Assert            
            await Assert.ThrowsAsync<Exception>(() => sut.PostAsync(advertRequest));
        }
    }
}
