using FluentValidation;
using Tokengram.Database.Tokengram;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.WS.Requests.Validators
{
    public class ChatMessageDeleteRequestDTOValidator : BaseValidator<ChatMessageDeleteRequestDTO>
    {
        private readonly TokengramDbContext _dbContext;

        public ChatMessageDeleteRequestDTOValidator(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.ChatMessageId)
                .Must(chatMessageId => _dbContext.ChatMessages.Any(x => x.Id == chatMessageId));
        }
    }
}
