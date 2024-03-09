using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Tokengram.Hubs
{
    public class BaseHub<T> : Hub<T>
        where T : class
    {
        protected string GetUserAddress()
        {
            return Context.User!.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}
