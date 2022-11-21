using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;

namespace OrderTakingService.Controllers
{
    public class TableController : ApiController
    {
        public IHttpActionResult Get(string key)
        {
            try
            {
                if (!Snippets.Authenticate(key)) return Unauthorized();
                List<Table> list = GetTables();
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

        private List<Table> GetTables()
        {
            DataTable data = Database.ExecProc("uspApiGetTables", null) ?? new DataTable();
            List<Table> tables = (from DataRow r in data.Rows.Cast<DataRow>()
                                  select new Table
                                  {
                                      Id = r["id"].ToString(),
                                      Name = r["tables"].ToString(),
                                      Reserved = r["table_status"].ToString() == "Open",
                                  }).ToList();
            return tables;
        }
    }
}
