using FluentValidation;
using Nethereum.Util;

namespace Tokengram.DTOS.Requests.Validators
{
    public class NonceRequestDTOValidator : BaseValidator<NonceRequestDTO>
    {
        public NonceRequestDTOValidator()
        {
            RuleFor(x => x.PublicAddress).NotEmpty().Must(AddressUtil.Current.IsValidEthereumAddressHexFormat);
        }
    }
}
