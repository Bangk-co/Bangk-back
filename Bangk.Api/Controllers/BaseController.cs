using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace Bangk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        public IHttpContextAccessor HttpContextAccessor { get; }
        protected readonly int UserId;

        public BaseController()
        {
        }

        public BaseController(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;

            var claims = httpContextAccessor?.HttpContext?.User?.Claims?.ToList();
            if (claims != null && claims.Any())
            {
                int.TryParse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value, out UserId);
            }
        }
    }
}
