using FluentValidation;
using Nethereum.Util;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.HTTP.Requests.Validators
{
    public class NonceRequestDTOValidator : BaseValidator<NonceRequestDTO>
    {
        public NonceRequestDTOValidator()
        {
            RuleFor(x => x.Address).NotEmpty().Must(AddressUtil.Current.IsValidEthereumAddressHexFormat);
        }
    }
}
