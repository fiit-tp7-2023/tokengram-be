using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Tokengram.Database.Tokengram;
using Tokengram.Database.Tokengram.Entities;
using Tokengram.Infrastructure.ActionFilterAttributes;
using Tokengram.Models.Exceptions;

namespace Tokengram.Infrastructure.HubFilters
{
    public class BindUserHubFilter : IHubFilter
    {
        private readonly TokengramDbContext _dbContext;

        public BindUserHubFilter(TokengramDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object?>> next
        )
        {
            var bindUserHubAttribute = (BindUserHubAttribute?)
                Attribute.GetCustomAttribute(invocationContext.HubMethod, typeof(BindUserHubAttribute));

            if (bindUserHubAttribute == null)
                return await next(invocationContext);

            var methodParameters = invocationContext.HubMethod.GetParameters();
            var argumentValues = invocationContext.HubMethodArguments;
            var parametersWithValues = new Dictionary<string, object>();

            for (int i = 0; i < methodParameters.Length; i++)
                parametersWithValues.Add(methodParameters[i].Name!, argumentValues[i]!);

            parametersWithValues.TryGetValue(bindUserHubAttribute.UserMethodKey, out var userAddressObj);

            if (string.IsNullOrEmpty(userAddressObj?.ToString()))
                throw new BadRequestException(Constants.ErrorMessages.INVALID_ARGUMENTS);

            User user =
                await _dbContext.Users.FirstOrDefaultAsync(x => x.Address == userAddressObj.ToString())
                ?? throw new NotFoundException(Constants.ErrorMessages.USER_NOT_FOUND);

            invocationContext.Context.Items[bindUserHubAttribute.ItemKey] = user;

            return await next(invocationContext);
        }
    }
}
