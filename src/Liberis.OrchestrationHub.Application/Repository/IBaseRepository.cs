using System.Collections.Generic;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Application.Repository
{
    public interface IBaseRepository<T> 
    {
        void CreateAsync(T obj);
        void UpdateByIdAsync(T obj);
        Task<T> GetByIdAsync(string RequestId, string collectionName);
    }
}
