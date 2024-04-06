using Microsoft.AspNetCore.Mvc.Filters;

namespace Tokengram.Infrastructure.ActionFilterAttributes
{
    public class BindChatMessageHubAttribute : ActionFilterAttribute
    {
        public string ChatMethodKey = "chatId";

        public string ChatMessageMethodKey = "chatMessageId";

        public string ItemKey = "chatMessage";
    }
}
