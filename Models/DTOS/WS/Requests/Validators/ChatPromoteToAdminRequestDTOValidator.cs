using FluentValidation;
using Tokengram.Database.Postgres;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.WS.Requests.Validators
{
    public class ChatPromoteToAdminRequestDTOValidator : BaseValidator<ChatPromoteToAdminRequestDTO>
    {
        private readonly TokengramDbContext _dbContext;

        public ChatPromoteToAdminRequestDTOValidator(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.ChatId).Must(chatId => _dbContext.Chats.Any(x => x.Id == chatId));
            RuleFor(x => x.AdminAddress).Must(address => _dbContext.Users.Any(x => x.Address == address));
        }
    }
}
