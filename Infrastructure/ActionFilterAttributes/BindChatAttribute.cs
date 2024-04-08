using Microsoft.AspNetCore.Mvc.Filters;
using Tokengram.Database.Tokengram;
using Tokengram.Models.Exceptions;
using Tokengram.Database.Tokengram.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tokengram.Infrastructure.ActionFilters
{
    public class BindChatAttribute : ActionFilterAttribute
    {
        public string ChatRouteKey = "chatId";

        public string ItemKey = "chat";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            TokengramDbContext dbContext = context.HttpContext.RequestServices.GetRequiredService<TokengramDbContext>();

            context.ActionArguments.TryGetValue(ChatRouteKey, out var chatIdObj);

            if (!long.TryParse(chatIdObj?.ToString(), out var chatId))
                throw new BadRequestException(Constants.ErrorMessages.INVALID_ARGUMENTS);

            Chat chat =
                await dbContext.Chats.FirstOrDefaultAsync(x => x.Id == chatId)
                ?? throw new NotFoundException(Constants.ErrorMessages.CHAT_NOT_FOUND);

            context.HttpContext.Items[ItemKey] = chat;

            await next();
        }
    }
}
