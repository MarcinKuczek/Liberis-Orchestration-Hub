namespace Liberis.OrchestrationHub.Core.Interfaces
{
    public interface IAdapterNameProvider<T>
    {
        string GetAdapterName(T request);
    }
}
