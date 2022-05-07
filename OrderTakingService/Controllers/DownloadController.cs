using OrderTakingService.Lib;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OrderTakingService.Controllers
{
    public class DownloadController : ApiController
    {
        public HttpResponseMessage Get(string key, string param)
        {
            try
            {
                if (string.IsNullOrEmpty(param)) return Request.CreateResponse(HttpStatusCode.BadRequest, "Param is missing");
                if(!Snippets.Authenticate(key)) return Request.CreateResponse(HttpStatusCode.Unauthorized);

                switch (param)
                {
                    case "apk":
                        
                            string path = Lib.Database.GetStringData("select [value] from api_settings where [label] = 'apk_path'");

                            if (path == null)
                            {
                                path = @"D:\Devaj\App\app-release.apk";
                            }

                            byte[] dataBytes = File.ReadAllBytes(path);
                            MemoryStream dataStream = new MemoryStream(dataBytes);

                            HttpResponseMessage httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK);
                            httpResponseMessage.Content = new StreamContent(dataStream);
                            httpResponseMessage.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                            {
                                FileName = Path.GetFileName(path)
                            };
                            httpResponseMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                            return httpResponseMessage;
                        
                    default:
                        return new HttpResponseMessage(HttpStatusCode.Unauthorized);
                }
            }
            catch (System.Exception)
            {

                return new HttpResponseMessage(HttpStatusCode.BadRequest);
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
