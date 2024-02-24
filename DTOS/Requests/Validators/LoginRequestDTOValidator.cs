using FluentValidation;
using Nethereum.Util;

namespace Tokengram.DTOS.Requests.Validators
{
    public class LoginRequestDTOValidator : BaseValidator<LoginRequestDTO>
    {
        public LoginRequestDTOValidator()
        {
            RuleFor(x => x.Signature).NotEmpty();

            RuleFor(x => x.PublicAddress).NotEmpty().Must(AddressUtil.Current.IsValidEthereumAddressHexFormat);
        }
    }
}
