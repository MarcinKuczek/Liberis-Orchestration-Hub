using Liberis.Common.Test.Builders;
using Liberis.OrchestrationHub.Core.Models;

namespace Liberis.OrchestrationHub.Tests.Common.Builders
{
    public sealed class ExampleResponseBuilder : Builder<ExampleResponse>
    {
        public ExampleResponseBuilder WithMessage(string message)
        {
            Instance.Message = message;
            return this;
        }

        public ExampleResponseBuilder WithSuccess(bool success)
        {
            Instance.Successful = success;
            return this;
        }

        public override ExampleResponse Build()
        {
            var instance = Instance;
            
            Instance = new ExampleResponse();
            
            return instance;
        }
    }
}