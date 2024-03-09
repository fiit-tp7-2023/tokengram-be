using FluentValidation;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class RefreshTokenRequestDTOValidator : BaseValidator<RefreshTokenRequestDTO>
    {
        public RefreshTokenRequestDTOValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
