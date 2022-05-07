using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Data;
using System.Web.Http;
using System.Web.Http.Results;

namespace OrderTakingService.Controllers
{
    public class LoginController : ApiController
    {
        public IHttpActionResult Get(string key, string username, string password, string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(username)) return BadRequest("Username is missing");
                if (string.IsNullOrEmpty(password)) return BadRequest("Password is missing");
                if (string.IsNullOrEmpty(deviceId)) return BadRequest("DeviceId is missing");
                if (!Snippets.Authenticate(key)) return Unauthorized();

                DataTable data = Database.ExecProc("uspApiGetUser", new string[] { username, password, deviceId });
                if (data != null && data.Rows[0][0].ToString().Equals("2"))
                {
                    return BadRequest("No shift is currently open. Please open shift first.");
                }
                else if(data != null && data.Rows[0][0].ToString().Equals("-1"))
                {
                    return BadRequest("No Such User Exists");
                }
                else if (data != null && data.Rows.Count >= 1)
                {
                    return Ok( new User
                    {
                        Id = data.Rows[0][0].ToString(),
                        Name = data.Rows[0][1].ToString(),
                        TiltId = data.Rows[0][2].ToString()
                    });
                }
                else
                {
                    return BadRequest("No Such User Exists");
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public IHttpActionResult Post(string key)
        {
            if (!Snippets.Authenticate(key)) return Unauthorized();
            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }

        public IHttpActionResult Put(string key)
        {
            if (!Snippets.Authenticate(key)) return Unauthorized();
            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }

        public IHttpActionResult Delete(string key)
        {
            if (!Snippets.Authenticate(key)) return Unauthorized();
            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }
    }
}
