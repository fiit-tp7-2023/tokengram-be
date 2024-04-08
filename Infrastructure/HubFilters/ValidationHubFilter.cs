using FluentValidation;
using Microsoft.AspNetCore.SignalR;

namespace Tokengram.Infrastructure.HubFilters
{
    public class ValidationHubFilter : IHubFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationHubFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next
        )
        {
            var requestType = invocationContext.HubMethodArguments[0]?.GetType();

            if (requestType != null)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(requestType);

                if (_serviceProvider.GetService(validatorType) is IValidator validator)
                {
                    var request = invocationContext.HubMethodArguments[0];
                    var validationResult = await validator.ValidateAsync(new ValidationContext<object>(request!));

                    if (!validationResult.IsValid)
                        throw new ValidationException(
                            Constants.ErrorMessages.VALIDATION_ERROR,
                            validationResult.Errors
                        );
                }
            }

            return await next(invocationContext);
        }
    }
}
