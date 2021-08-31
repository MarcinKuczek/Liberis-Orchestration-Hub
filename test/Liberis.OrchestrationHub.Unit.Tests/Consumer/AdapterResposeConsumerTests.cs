using Liberis.OrchestrationAdapter.Messages.V1;
using Liberis.OrchestrationAdapter.Messages.V1.Advert;
using Liberis.OrchestrationHub.Application.Consumer;
using Liberis.OrchestrationHub.Application.Repository;
using MassTransit;
using Moq;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace Liberis.OrchestrationHub.Unit.Tests.Consumer
{
    public class AdapterResposeConsumerTests
    {
        private readonly IPublishEndpoint _mockPublishEndpoint;

        public AdapterResposeConsumerTests()
        {
            _mockPublishEndpoint = Substitute.For<IPublishEndpoint>();
        }

        private static AdapterResponseConsumer<ExternalAdvertResponse> Arrange(AdapterResponse<ExternalAdvertResponse> obj)
        {
            var mockRepository = Substitute.For<IBaseRepository<AdapterResponse<ExternalAdvertResponse>>>();
            mockRepository.CreateAsync(obj);

            return new AdapterResponseConsumer<ExternalAdvertResponse>(mockRepository);
        }

        [Fact]
        public void Consume_SuccessfulMessageBrokerConsumption_PublishExternalAdvertResponse()
        {
            // Arrange
            var externalAdvert = new ExternalAdvertResponse
            {
                ExternalAdvertResponseData = new ExternalAdvertResponseData { Body = "test", Subtitle = "test" },
                Meta = new ExternalAdvertMeta { Reference = "test" }
            };

            var obj = new AdapterResponse<ExternalAdvertResponse>
            {
                Response = externalAdvert,
                AdapterName = "test",
                RequestId = "123456789"
            };

            var sut = Arrange(obj);
            _mockPublishEndpoint.Publish<AdapterResponse<ExternalAdvertResponse>>(obj).Returns(Task.CompletedTask);
            var context = Mock.Of<ConsumeContext<AdapterResponse<ExternalAdvertResponse>>>(_ => _.Message == obj);

            // Act
            var result = sut.Consume(context);

            //Assert
            Assert.True(result.IsCompleted);
        }

    }
}
