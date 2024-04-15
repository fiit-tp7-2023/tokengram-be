using FluentValidation;
using Tokengram.Database.Tokengram;
using Tokengram.Models.Validation;

namespace Tokengram.Models.DTOS.WS.Requests.Validators
{
    public class ChatMessageRequestDTOValidator : BaseValidator<ChatMessageRequestDTO>
    {
        private readonly TokengramDbContext _dbContext;

        public ChatMessageRequestDTOValidator(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.Content).NotEmpty().MaximumLength(1000);
            When(
                x => x.ParentMessageId != null,
                () =>
                {
                    RuleFor(x => x.ParentMessageId)
                        .Must(parentMessageId => _dbContext.ChatMessages.Any(x => x.Id == parentMessageId));
                }
            );
        }
    }
}
