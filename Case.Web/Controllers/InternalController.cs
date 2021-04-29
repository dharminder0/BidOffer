using Case.Business;
using Cases.Web.Api.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cases.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalController : ControllerBase
    {
        [HttpGet]
        [CcsAuthorize(ValidationRequired = false)]
        [Route("GenerateToken")]
        public object GenerateToken(string param)
        {
            return JwtSecurityService.Encrypt(JwtSecurityService.BuildJwtToken(param, 10));
        }
    }
}
