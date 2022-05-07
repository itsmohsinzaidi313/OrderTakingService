using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace OrderTakingService.Controllers
{
    public class UserController : ApiController
    {
        public IHttpActionResult Get(string key)
        {
            try
            {
                if (!Snippets.Authenticate(key)) return Unauthorized();

                DataTable dt = Database.ExecProc("uspApiGetAllUsers", null) ?? new DataTable();
                List<User> users = (from DataRow dr in dt.Rows.Cast<DataRow>()
                                    select new User
                                    {
                                        Id = dr["id"].ToString(),
                                        Name = dr["name"].ToString(),
                                        TiltId = dr["tiltId"].ToString()
                                    }).ToList();
                if (users.Count >= 1)
                {
                    return Ok(users);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }
    }
}
