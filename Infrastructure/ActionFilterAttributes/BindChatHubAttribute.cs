using Microsoft.AspNetCore.Mvc.Filters;

namespace Tokengram.Infrastructure.ActionFilterAttributes
{
    public class BindChatHubAttribute : ActionFilterAttribute
    {
        public string ChatMethodKey = "chatId";

        public string ItemKey = "chat";
    }
}
