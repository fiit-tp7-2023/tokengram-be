using System.Text.Json;
using FluentValidation;
using Tokengram.Models.Validation;

namespace Tokengram.Utils
{
    public class ErrorUtil
    {
        public static IEnumerable<ValidationError> BuildValidationErrors(ValidationException ex)
        {
            var errors = new List<ValidationError>();

            foreach (var error in ex.Errors)
                errors.Add(
                    new ValidationError
                    {
                        Property = JsonNamingPolicy.CamelCase.ConvertName(error.PropertyName),
                        Message = error.ErrorMessage
                    }
                );

            return errors;
        }
    }
}
