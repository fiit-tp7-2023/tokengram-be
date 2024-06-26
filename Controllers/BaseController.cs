using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Tokengram.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        protected string GetUserAddress()
        {
            return User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        }

        [NonAction]
        protected bool IsUserLoggedIn()
        {
            return User.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier);
        }
    }
}
