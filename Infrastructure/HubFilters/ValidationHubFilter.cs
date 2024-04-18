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
            foreach (object? argument in invocationContext.HubMethodArguments)
            {
                var argumentType = argument?.GetType();

                if (argumentType != null)
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                    if (_serviceProvider.GetService(validatorType) is IValidator validator)
                    {
                        var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument!));

                        if (!validationResult.IsValid)
                            throw new ValidationException(
                                Constants.ErrorMessages.VALIDATION_ERROR,
                                validationResult.Errors
                            );
                    }
                }
            }

            return await next(invocationContext);
        }
    }
}
