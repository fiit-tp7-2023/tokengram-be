using Microsoft.AspNetCore.Mvc.Filters;
using Tokengram.Database.Tokengram;
using Tokengram.Models.Exceptions;
using Tokengram.Database.Tokengram.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tokengram.Infrastructure.ActionFilters
{
    public class BindPostAttribute : ActionFilterAttribute
    {
        public string PostRouteKey = "postNFTAddress";

        public string ItemKey = "post";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            TokengramDbContext dbContext = context.HttpContext.RequestServices.GetRequiredService<TokengramDbContext>();

            context.ActionArguments.TryGetValue(PostRouteKey, out var postNFTAddressObj);

            if (string.IsNullOrEmpty(postNFTAddressObj?.ToString()))
                throw new BadRequestException(Constants.ErrorMessages.INVALID_ARGUMENTS);

            Post post =
                await dbContext.Posts.FirstOrDefaultAsync(x => x.NFTAddress == postNFTAddressObj.ToString())
                ?? throw new NotFoundException(Constants.ErrorMessages.POST_NOT_FOUND);

            context.HttpContext.Items[ItemKey] = post;

            await next();
        }
    }
}
