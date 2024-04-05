using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Infrastructure.ActionFilterAttributes;
using Tokengram.Models.Exceptions;

namespace Tokengram.Infrastructure.HubFilters
{
    public class BindChatMessageHubFilter : IHubFilter
    {
        private readonly TokengramDbContext _dbContext;

        public BindChatMessageHubFilter(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next
        )
        {
            var bindChatMessageHubAttribute = (BindChatMessageHubAttribute?)
                Attribute.GetCustomAttribute(invocationContext.HubMethod, typeof(BindChatMessageHubAttribute));

            if (bindChatMessageHubAttribute == null)
                return await next(invocationContext);

            var methodParameters = invocationContext.HubMethod.GetParameters();
            var argumentValues = invocationContext.HubMethodArguments;
            var parametersWithValues = new Dictionary<string, object>();

            for (int i = 0; i < methodParameters.Length; i++)
                parametersWithValues.Add(methodParameters[i].Name!, argumentValues[i]!);

            parametersWithValues.TryGetValue(bindChatMessageHubAttribute.ChatMethodKey, out var chatIdObj);
            parametersWithValues.TryGetValue(
                bindChatMessageHubAttribute.ChatMessageMethodKey,
                out var chatMessageIdObj
            );

            if (
                !long.TryParse(chatIdObj?.ToString(), out var chatId)
                || !long.TryParse(chatMessageIdObj?.ToString(), out var chatMessageId)
            )
                throw new BadRequestException(Constants.ErrorMessages.INVALID_ARGUMENTS);

            ChatMessage chatMessage =
                await _dbContext.ChatMessages.FirstOrDefaultAsync(x => x.Id == chatMessageId && x.ChatId == chatId)
                ?? throw new NotFoundException(Constants.ErrorMessages.CHAT_MESSAGE_NOT_FOUND);

            invocationContext.Context.Items[bindChatMessageHubAttribute.ItemKey] = chatMessage;

            return await next(invocationContext);
        }
    }
}
