using FluentValidation;
using Tokengram.Database.Tokengram;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.WS.Requests.Validators
{
    public class ChatLeaveRequestDTOValidator : BaseValidator<ChatLeaveRequestDTO>
    {
        private readonly TokengramDbContext _dbContext;

        public ChatLeaveRequestDTOValidator(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.ChatId).Must(chatId => _dbContext.Chats.Any(x => x.Id == chatId));
        }
    }
}
