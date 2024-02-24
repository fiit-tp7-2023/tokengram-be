using FluentValidation;
using FluentValidation.Results;

namespace Tokengram.DTOS.Requests.Validators;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    public override ValidationResult Validate(ValidationContext<T> context)
    {
        ValidationResult validationResult = base.Validate(context);

        if (!validationResult.IsValid)
        {
            RaiseValidationException(context, validationResult);
        }

        return validationResult;
    }
}
