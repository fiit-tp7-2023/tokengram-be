using Microsoft.AspNetCore.Mvc.Filters;
using Tokengram.Database.Tokengram;
using Tokengram.Models.Exceptions;
using Tokengram.Database.Tokengram.Entities;
using Microsoft.EntityFrameworkCore;

namespace Tokengram.Infrastructure.ActionFilters
{
    public class BindUserAttribute : ActionFilterAttribute
    {
        public string RouteKey = "userAddress";

        public string ItemKey = "user";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            TokengramDbContext dbContext = context.HttpContext.RequestServices.GetRequiredService<TokengramDbContext>();

            context.ActionArguments.TryGetValue(RouteKey, out var userAddressObj);

            if (string.IsNullOrEmpty(userAddressObj?.ToString()))
                throw new BadRequestException(Constants.ErrorMessages.INVALID_ARGUMENTS);

            User user =
                await dbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddressObj.ToString())
                ?? throw new NotFoundException(Constants.ErrorMessages.USER_NOT_FOUND);

            context.HttpContext.Items[ItemKey] = user;

            await next();
        }
    }
}
