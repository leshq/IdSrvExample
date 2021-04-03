using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace IdSrvExample.Services.Assets.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthTestController : ControllerBase
    {
        private readonly ILogger<AuthTestController> _logger;

        public AuthTestController(ILogger<AuthTestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("public")]
        public IActionResult Public()
        {
            return Ok("Assets service: public data");
        }

        [HttpGet]
        [Authorize]
        [Route("private")]
        public IActionResult Private()
        {
            var result = new
            {
                Page = "Assets service: private data",
                Data = from c in User.Claims select new { c.Type, c.Value },
                UserIdentity = User.Identity.Name
            };

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        [Route("admin")]
        public IActionResult Secure()
        {
            var result = new
            {
                Page = "Assets service: admin data",
                Data = from c in User.Claims select new { c.Type, c.Value },
                UserIdentity = User.Identity.Name
            };

            return Ok(result);
        }
    }
}
