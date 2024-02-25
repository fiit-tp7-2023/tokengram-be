using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Tokengram.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public long GetUserId()
        {
            return long.Parse(User.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
        }

        [NonAction]
        public string GetPublicAddress()
        {
            return User.Claims.First(x => x.Type == ClaimTypes.Actor).Value;
        }
    }
}
