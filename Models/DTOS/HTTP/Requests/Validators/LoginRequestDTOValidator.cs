using FluentValidation;
using Nethereum.Util;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class LoginRequestDTOValidator : BaseValidator<LoginRequestDTO>
    {
        public LoginRequestDTOValidator()
        {
            RuleFor(x => x.Signature).NotEmpty();

            RuleFor(x => x.Address).NotEmpty().Must(AddressUtil.Current.IsValidEthereumAddressHexFormat);
        }
    }
}
