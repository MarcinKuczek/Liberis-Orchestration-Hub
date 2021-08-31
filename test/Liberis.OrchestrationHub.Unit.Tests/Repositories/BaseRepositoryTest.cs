using Liberis.OrchestrationAdapter.Messages.V1;
using Liberis.OrchestrationAdapter.Messages.V1.Advert;
using Liberis.OrchestrationHub.Application.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace Liberis.OrchestrationHub.Unit.Tests.Repositories
{
    public class BaseRepositoryTest
    {
        private static BaseRepository<object> Arrange(IMongoDatabase mongodb)
        {
            return new BaseRepository<object>(mongodb);
        }

        [Fact]
        public void CreateAsync_GivenValidObject_ReturnTrue()
        {
            var mongodb = Substitute.For<IMongoDatabase>();
            var externalAdvert = new ExternalAdvertResponse
            {
                ExternalAdvertResponseData = new ExternalAdvertResponseData { Body = "test", Subtitle = "test" },
                Meta = new ExternalAdvertMeta { Reference = "test" }
            };
            var obj = new AdapterResponse<object>
            {
                Response = externalAdvert,
                AdapterName = "test",
                RequestId = "123456789"
            };
            var collection = Substitute.For<IMongoCollection<AdapterResponse<object>>>();
            mongodb.GetCollection<AdapterResponse<object>>(obj.AdapterName).Returns(collection);
            var sut = Arrange(mongodb);

            sut.CreateAsync(obj);

            mongodb.Received().GetCollection<AdapterResponse<object>>(obj.AdapterName);
        }

        [Fact]
        public void UpdateByIdAsync_GivenValidObject_ReturnTrue()
        {
            var mongodb = Substitute.For<IMongoDatabase>();
            var externalAdvert = new ExternalAdvertResponse
            {
                ExternalAdvertResponseData = new ExternalAdvertResponseData { Body = "test", Subtitle = "test" },
                Meta = new ExternalAdvertMeta { Reference = "test" }
            };

            var obj = new AdapterResponse<object>
            {
                Response = externalAdvert,
                AdapterName = "test",
                RequestId = "123456789"
            };
            var collection = Substitute.For<IMongoCollection<AdapterResponse<object>>>();
            mongodb.GetCollection<AdapterResponse<object>>(obj.AdapterName).Returns(collection);

            var sut = Arrange(mongodb);

            sut.UpdateByIdAsync(obj);

            mongodb.Received().GetCollection<AdapterResponse<object>>(obj.AdapterName);
        }

        [Fact]
        public async void GetByIdAsync_GivenValidValues_ReturnTrue()
        {
            var mongodb = Substitute.For<IMongoDatabase>();
            var externalAdvert = new ExternalAdvertResponse
            {
                ExternalAdvertResponseData = new ExternalAdvertResponseData { Body = "test", Subtitle = "test" },
                Meta = new ExternalAdvertMeta { Reference = "test" }
            };

            var obj = new AdapterResponse<object>
            {
                Response = externalAdvert,
                AdapterName = "test",
                RequestId = "123456789"
            };
            var collectionName = "test";
            var filter = Builders<AdapterResponse<object>>.Filter.Eq(x => x.RequestId, obj.RequestId);
            var collection = Substitute.For<IMongoCollection<AdapterResponse<object>>>();
            mongodb.GetCollection<AdapterResponse<object>>(collectionName).Returns(collection);

            var sut = Arrange(mongodb);

           var result = await sut.GetByIdAsync(obj.RequestId,collectionName);

            mongodb.Received().GetCollection<AdapterResponse<object>>(collectionName);
        }
    }
}
