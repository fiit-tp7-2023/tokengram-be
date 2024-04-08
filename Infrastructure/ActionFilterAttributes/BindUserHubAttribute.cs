using Microsoft.AspNetCore.Mvc.Filters;

namespace Tokengram.Infrastructure.ActionFilterAttributes
{
    public class BindUserHubAttribute : ActionFilterAttribute
    {
        public string UserMethodKey = "userAddress";

        public string ItemKey = "user";
    }
}
