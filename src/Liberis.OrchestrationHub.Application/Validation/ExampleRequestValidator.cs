using FluentValidation;
using Liberis.OrchestrationHub.Core.Models;

namespace Liberis.OrchestrationHub.Application.Validation
{
    public class ExampleRequestValidator : AbstractValidator<ExampleRequest>
    {
        public ExampleRequestValidator()
        {
            RuleFor(x => x.Example).NotEmpty();
            RuleFor(x => x.Example).NotNull();
        }
    }
}
