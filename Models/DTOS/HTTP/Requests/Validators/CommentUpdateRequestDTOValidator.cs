using FluentValidation;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class CommentUpdateRequestDTOValidator : BaseValidator<CommentUpdateRequestDTO>
    {
        public CommentUpdateRequestDTOValidator()
        {
            RuleFor(x => x.Content).NotEmpty().MaximumLength(500);
        }
    }
}
