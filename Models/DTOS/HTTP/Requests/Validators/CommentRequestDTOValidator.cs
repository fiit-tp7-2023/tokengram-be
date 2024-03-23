using FluentValidation;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class CommentRequestDTOValidator : BaseValidator<CommentRequestDTO>
    {
        public CommentRequestDTOValidator()
        {
            RuleFor(x => x.Content).NotEmpty().MaximumLength(500);
            When(
                x => x.ParentCommentId != null,
                () =>
                {
                    RuleFor(x => x.ParentCommentId).GreaterThanOrEqualTo(0);
                }
            );
        }
    }
}
