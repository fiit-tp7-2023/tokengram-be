using Microsoft.AspNetCore.Mvc.Filters;
using Tokengram.Database.Tokengram;
using Tokengram.Models.Exceptions;
using Tokengram.Database.Tokengram.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tokengram.Infrastructure.ActionFilters
{
    public class BindCommentAttribute : ActionFilterAttribute
    {
        public string PostRouteKey = "postNFTAddress";

        public string CommentRouteKey = "commentId";

        public string ItemKey = "comment";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            TokengramDbContext dbContext = context.HttpContext.RequestServices.GetRequiredService<TokengramDbContext>();

            context.ActionArguments.TryGetValue(PostRouteKey, out var postNFTAddressObj);
            context.ActionArguments.TryGetValue(CommentRouteKey, out var commentIdObj);

            if (
                string.IsNullOrEmpty(postNFTAddressObj?.ToString())
                || !long.TryParse(commentIdObj?.ToString(), out long commentId)
            )
                throw new BadRequestException(Constants.ErrorMessages.INVALID_ARGUMENTS);

            Comment comment =
                await dbContext.Comments.FirstOrDefaultAsync(
                    x => x.PostNFTAddress == postNFTAddressObj.ToString() && x.Id == commentId
                ) ?? throw new NotFoundException(Constants.ErrorMessages.COMMENT_NOT_FOUND);

            context.HttpContext.Items[ItemKey] = comment;

            await next();
        }
    }
}
