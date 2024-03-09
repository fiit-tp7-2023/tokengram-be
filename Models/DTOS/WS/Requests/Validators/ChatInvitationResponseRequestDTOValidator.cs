using FluentValidation;
using Tokengram.Database.Postgres;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.WS.Requests.Validators
{
    public class ChatInvitationResponseRequestDTOValidator : BaseValidator<ChatInvitationResponseRequestDTO>
    {
        private readonly TokengramDbContext _dbContext;

        public ChatInvitationResponseRequestDTOValidator(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.ChatId).Must(chatId => _dbContext.Chats.Any(x => x.Id == chatId));
        }
    }
}
