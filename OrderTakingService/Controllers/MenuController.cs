using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace OrderTakingService.Controllers
{
    public class MenuController : ApiController
    {
        public IHttpActionResult Get(string key, string phrase)
        {
            try
            {
                if (string.IsNullOrEmpty(phrase))
                {
                    return BadRequest("Phrase is missing");
                }

                if (!Snippets.Authenticate(key))
                {
                    return Unauthorized();
                }

                if ("*".Equals(phrase))
                {
                    Menu menu = GetMenu();

                    if (menu.IsValid)
                    {
                        return Ok(menu);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    Menu menu = GetMenu();
                    Menu searchResult = new Menu();
                    if (int.TryParse(phrase, out int code))
                    {
                        searchResult = new Menu
                        {
                            Categories = menu.Categories,
                            Items = (from x in menu.Items where x.Code.Contains(code.ToString()) select x).ToList(),
                            FixedDeals = (from x in menu.FixedDeals where x.Code.Contains(code.ToString()) select x).ToList(),
                            OnSpotDeals = (from x in menu.OnSpotDeals where x.Code.Contains(code.ToString()) select x).ToList(),
                        };
                    }
                    else
                    {
                        searchResult = new Menu
                        {
                            Categories = menu.Categories,
                            Items = (from x in menu.Items where x.Name.Contains(phrase) select x).ToList(),
                            FixedDeals = (from x in menu.FixedDeals where x.Name.Contains(phrase) select x).ToList(),
                            OnSpotDeals = (from x in menu.OnSpotDeals where x.Name.Contains(phrase) select x).ToList(),
                        };
                    }

                    if ((searchResult.FixedDeals.Count + searchResult.Items.Count + searchResult.OnSpotDeals.Count) >= 1)
                    {
                        return Ok(menu);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public IHttpActionResult Post(string key)
        {
            if (!Snippets.Authenticate(key))
            {
                return Unauthorized();
            }

            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }

        public IHttpActionResult Put(string key)
        {
            if (!Snippets.Authenticate(key))
            {
                return Unauthorized();
            }

            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }

        public IHttpActionResult Delete(string key)
        {
            if (!Snippets.Authenticate(key))
            {
                return Unauthorized();
            }

            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }


        private Menu GetMenu()
        {
            Menu menu = new Menu();
            string xml = Database.GetStringData("exec uspApiPOSGetMenu");
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Menu));
            byte[] vs = Encoding.UTF8.GetBytes(xml);

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(vs))
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                menu = (Menu)x.Deserialize(xmlReader);
            }
            return menu;
        }
    }
}
