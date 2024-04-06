using FluentValidation;
using Tokengram.Constants;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class FollowRequestDTOValidator : BaseValidator<FollowRequestDTO>
    {
        public FollowRequestDTOValidator() {
            RuleFor(x => x.UserAddress).MaximumLength(ProfileSettings.MAX_ADDRESS_LENGTH);
        }
    }
}
