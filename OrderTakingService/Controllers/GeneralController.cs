using OrderTakingService.Models;
using System.Web.Http;
using System.Web.Http.Results;

namespace OrderTakingService.Controllers
{
    public class GeneralController : ApiController
    {
        [Route("api/pos/Status")]
        public IHttpActionResult Get(string key)
        {
            if (!Lib.Snippets.Authenticate(key)) return Unauthorized();
            return Ok();
        }
    }
}
