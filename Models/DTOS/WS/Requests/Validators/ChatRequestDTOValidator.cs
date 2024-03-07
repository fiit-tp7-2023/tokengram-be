using FluentValidation;
using Tokengram.Database.Postgres;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.WS.Requests.Validators
{
    public class ChatRequestDTOValidator : BaseValidator<ChatRequestDTO>
    {
        private readonly TokengramDbContext _dbContext;

        public ChatRequestDTOValidator(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.UserAddresses)
                .NotEmpty()
                .Must(userAddresses => userAddresses.Distinct().Count() == userAddresses.Count)
                .Must(userAddresses => userAddresses.All(address => _dbContext.Users.Any(u => u.Address == address)));
        }
    }
}
