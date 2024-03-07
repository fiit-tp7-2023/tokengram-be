using FluentValidation;
using Tokengram.Database.Postgres;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.WS.Requests.Validators
{
    public class ChatInvitationRequestDTOValidator : BaseValidator<ChatInvitationRequestDTO>
    {
        private readonly TokengramDbContext _dbContext;

        public ChatInvitationRequestDTOValidator(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.ChatId).Must(chatId => _dbContext.Chats.Any(x => x.Id == chatId));
            RuleFor(x => x.UserAddress).Must(userAddress => _dbContext.Users.Any(x => x.Address == userAddress));
        }
    }
}
