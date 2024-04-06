using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Infrastructure.ActionFilterAttributes;
using Tokengram.Models.Exceptions;

namespace Tokengram.Infrastructure.HubFilters
{
    public class BindChatHubFilter : IHubFilter
    {
        private readonly TokengramDbContext _dbContext;

        public BindChatHubFilter(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next
        )
        {
            var bindChatHubAttribute = (BindChatHubAttribute?)
                Attribute.GetCustomAttribute(invocationContext.HubMethod, typeof(BindChatHubAttribute));

            if (bindChatHubAttribute == null)
                return await next(invocationContext);

            var methodParameters = invocationContext.HubMethod.GetParameters();
            var argumentValues = invocationContext.HubMethodArguments;
            var parametersWithValues = new Dictionary<string, object>();

            for (int i = 0; i < methodParameters.Length; i++)
                parametersWithValues.Add(methodParameters[i].Name!, argumentValues[i]!);

            parametersWithValues.TryGetValue(bindChatHubAttribute.ChatMethodKey, out var chatIdObj);

            if (!long.TryParse(chatIdObj?.ToString(), out var chatId))
                throw new BadRequestException(Constants.ErrorMessages.INVALID_ARGUMENTS);

            Chat chat =
                await _dbContext.Chats.FirstOrDefaultAsync(x => x.Id == chatId)
                ?? throw new NotFoundException(Constants.ErrorMessages.CHAT_NOT_FOUND);

            invocationContext.Context.Items[bindChatHubAttribute.ItemKey] = chat;

            return await next(invocationContext);
        }
    }
}
