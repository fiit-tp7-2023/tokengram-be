using FluentValidation;

namespace Tokengram.DTOS.Requests.Validators
{
    public class RefreshTokenRequestDTOValidator : BaseValidator<RefreshTokenRequestDTO>
    {
        public RefreshTokenRequestDTOValidator()
        {
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
}
