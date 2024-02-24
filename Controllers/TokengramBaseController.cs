using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tokengram.Controllers
{
    public class TokengramBaseController : ControllerBase
    {
        [NonAction]
        public long GetUserId()
        {
            return long.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        }

        [NonAction]
        public string GetPublicAddress()
        {
            return User.Claims.First(x => x.Type == ClaimTypes.Actor).Value;
        }
    }
}
