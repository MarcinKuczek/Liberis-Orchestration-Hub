using Liberis.OrchestrationHub.Core.Models;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Core.Interfaces
{
    public interface IService
    {
        Task<ExampleResponse> GetAsync();
    }
}
