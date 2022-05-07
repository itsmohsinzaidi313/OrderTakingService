using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Data;
using System.Web.Http;
using System.Web.Http.Results;

namespace OrderTakingService.Controllers
{
    public class FeedbackController : ApiController
    {
        //public JsonResult<Transport> Get()
        //{
        //    Transport transport = new Transport();
        //    try
        //    {
        //        transport.Status = true;
        //        transport.Message = "Success";

        //    }
        //    catch(Exception e)
        //    {
        //        transport.Status = false;
        //        transport.Message = "Error";
        //        transport.Data = e.Message;
        //    }
        //    return Json(transport);
        //}
        public IHttpActionResult Get(string key)
        {
            if (!Snippets.Authenticate(key)) return Unauthorized();
            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }

        public IHttpActionResult Post(string key, Feedback feedback)
        {
            try
            {
                if (feedback == null) return BadRequest("Feedback is missing");
                if (!Snippets.Authenticate(key)) return Unauthorized();

                string xml = Lib.Snippets.SerializeToStringXml(feedback, feedback.GetType());
                xml = xml.Replace("\"", "");
                xml = xml.Replace("<?xml version=1.0 encoding=utf-8?>", "");
                DataTable dt = Lib.Database.ExecProc("uspApiInsertFeedback", new string[] { xml }) ?? new DataTable();
                int id = -1;
                if (int.TryParse(dt.Rows[0][0].ToString(), out id))
                {
                    return Created(id.ToString(), feedback);
                }
                else
                {
                    throw new Exception("Cannot create feedback");
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
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
