using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        protected string GetUserId()
        {
            var userClaimId = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            var userId = userClaimId.Value;
            return userId;
        }
    }
}
