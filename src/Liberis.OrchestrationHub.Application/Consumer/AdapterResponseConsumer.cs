using Liberis.OrchestrationAdapter.Messages.V1;
using Liberis.OrchestrationAdapter.Messages.V1.Advert;
using Liberis.OrchestrationHub.Application.Repository;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Liberis.OrchestrationHub.Application.Consumer
{
    public class AdapterResponseConsumer<T> : IConsumer<AdapterResponse<T>> where T : class
    {
        private readonly IBaseRepository<AdapterResponse<T>> _repository;

        public AdapterResponseConsumer(IBaseRepository<AdapterResponse<T>> repository)
        {
            _repository = repository;
        }

        public async Task Consume(ConsumeContext<AdapterResponse<T>> context)
        {
            AdapterResponse<T> adapterResponse = context.Message;
            _repository.CreateAsync(adapterResponse);
            await Task.CompletedTask;
        }
    }
}
