using FluentValidation;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class PostUserSettingsRequestDTOValidator : BaseValidator<PostUserSettingsRequestDTO>
    {
        public PostUserSettingsRequestDTOValidator()
        {
            When(
                x => x.Description != null,
                () =>
                {
                    RuleFor(x => x.Description).MaximumLength(500);
                }
            );
        }
    }
}
