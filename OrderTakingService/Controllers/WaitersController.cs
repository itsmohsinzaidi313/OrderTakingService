using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace OrderTakingService.Controllers
{
    public class WaitersController : ApiController
    {
        public IHttpActionResult Get(string key)
        {
            try
            {
                if (!Snippets.Authenticate(key))
                {
                    return Unauthorized();
                }

                DataTable data = Database.ExecProc("uspApiGetWaiters", null) ?? new DataTable();
                List<Waiter> list = (from DataRow r in data.Rows.Cast<DataRow>()
                                     select new Waiter
                                     {
                                         id = r["id"].ToString(),
                                         name = r["waiter_name"].ToString()
                                     }).ToList();
                if (list.Count >= 1)
                {
                    return Ok(list);
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
