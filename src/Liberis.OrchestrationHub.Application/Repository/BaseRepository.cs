using Liberis.OrchestrationAdapter.Messages.V1;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Application.Repository
{
    public class BaseRepository<T> : IBaseRepository<AdapterResponse<T>>
    {
        protected readonly IMongoDatabase _mongoDatabase;
        public BaseRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public async void CreateAsync(AdapterResponse<T> obj)
        {
            var collection = _mongoDatabase.GetCollection<AdapterResponse<T>>(obj.AdapterName);
            await collection.InsertOneAsync(obj);
        }

        public virtual async void UpdateByIdAsync(AdapterResponse<T> obj)
        {
            var filter = Builders<AdapterResponse<T>>.Filter.Eq(x => x.RequestId, obj.RequestId);
            var collection = _mongoDatabase.GetCollection<AdapterResponse<T>>(obj.AdapterName);
            var update = Builders<AdapterResponse<T>>.Update
                                              .Set(x => x.RequestId, obj.RequestId)
                                              .Set(x => x.Response, obj.Response);

            await collection.FindOneAndUpdateAsync(filter, update);
        }

        public virtual async Task<AdapterResponse<T>> GetByIdAsync(string requestId, string collectionName)
        {
            var filter = Builders<AdapterResponse<T>>.Filter.Eq(x => x.RequestId, requestId);
            var collection = _mongoDatabase.GetCollection<AdapterResponse<T>>(collectionName);
            var result = await collection.FindAsync(filter);
            return result.FirstOrDefault();
        }
    }
}