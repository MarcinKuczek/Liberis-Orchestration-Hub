using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Core.Interfaces
{
    public interface IHubService<T>
    {
        Task SendRequestToAdapter(T request);
    }
}
