using OrderTakingService.Lib;
using System;
using System.Web.Http;

namespace OrderTakingService.Controllers
{
    public class ReportController : ApiController
    {
        public IHttpActionResult Post(string key)
        {
            if (!Snippets.Authenticate(key)) return Unauthorized();
            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }
    }
}
