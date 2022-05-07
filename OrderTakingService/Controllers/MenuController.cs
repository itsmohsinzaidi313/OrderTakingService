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
                    //Menu menu = new Menu
                    //{
                    //    Items = GetItems(),
                    //    Categories = GetCategories(),
                    //    FixedDeals = GetFixedDeals(),
                    //    OnSpotDeals = GetOnSpotDeals()
                    //};

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
                    List<MenuItem> list = SearchItems(phrase);
                    if (list.Count >= 1)
                    {
                        return Ok(list);
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

        private List<Category> GetCategories()
        {
            DataTable data = Database.ExecProc("uspApiGetPOSCategories", null) ?? new DataTable();
            List<Category> categories = (from DataRow r in data.Rows.Cast<DataRow>()
                                         select new Category
                                         {
                                             Id = r["id"].ToString(),
                                             Name = r["category_name"].ToString(),
                                         }).ToList();
            return categories;
        }

        private List<MenuItem> GetItems()
        {
            DataTable data = Database.ExecProc("uspApiGetPOSAllItems", null) ?? new DataTable();
            List<MenuItem> items = (from DataRow r in data.Rows.Cast<DataRow>()
                                    select new MenuItem
                                    {
                                        Id = r["id"].ToString(),
                                        Code = r["codes"].ToString(),
                                        CategoryId = r["category_id"].ToString(),
                                        Name = r["item_name"].ToString(),
                                        Price = double.Parse(r["sale_price"].ToString()),
                                        TaxAmount = double.Parse(r["tax_price"].ToString()),
                                        Quantity = double.Parse(r["quantity"].ToString()),
                                        Comment = r["comment"].ToString(),
                                    }).ToList();
            return items;
        }

        private List<MenuItem> SearchItems(string phrase)
        {
            DataTable data = Database.ExecProc("uspApiSearchPOSItems", new string[] { phrase }) ?? new DataTable();
            List<MenuItem> list = (from DataRow r in data.Rows.Cast<DataRow>()
                                   select new MenuItem
                                   {
                                       Id = r["id"].ToString(),
                                       Code = r["codes"].ToString(),
                                       CategoryId = r["category_id"].ToString(),
                                       Name = r["item_name"].ToString(),
                                       Price = double.Parse(r["sale_price"].ToString()),
                                       TaxAmount = double.Parse(r["tax_price"].ToString()),
                                       Quantity = double.Parse(r["quantity"].ToString()),
                                       Comment = r["comment"].ToString(),
                                   }).ToList();
            return list;
        }

        private List<FixedDeal> GetFixedDeals()
        {
            DataTable data = Database.ExecProc("uspApiPOSGetAllFixedDeals", null) ?? new DataTable();

            string[] dealNames = (from DataRow x in data.Rows.Cast<DataRow>()
                                  select x["deal_name"].ToString())
                                  .ToArray()
                                  .Distinct()
                                  .ToArray();


            List<FixedDeal> deals = new List<FixedDeal>();
            foreach (string dealName in dealNames)
            {
                FixedDeal fixedDeal = (from DataRow r in data.Rows.Cast<DataRow>()
                                       where (r["deal_name"].ToString() == dealName)
                                       select new FixedDeal
                                       {
                                           Id = r["deal_id"].ToString(),
                                           CategoryId = r["deal_category_id"].ToString(),
                                           Code = r["deal_code"].ToString(),
                                           Name = r["deal_name"].ToString(),
                                           Price = double.Parse(r["deal_price"].ToString()),
                                           Comment = r["deal_comment"].ToString(),
                                           Quantity = double.Parse(r["deal_quantity"].ToString()),
                                           TaxAmount = double.Parse(r["deal_tax_price"].ToString()),
                                           Items = (from DataRow rr in data.Rows.Cast<DataRow>()
                                                    where rr["deal_name"].ToString().Equals(r["deal_name"].ToString())
                                                    select new MenuItem
                                                    {
                                                        Id = rr["id"].ToString(),
                                                        Code = rr["code"].ToString(),
                                                        CategoryId = rr["category_id"].ToString(),
                                                        Name = rr["item_name"].ToString(),
                                                        Price = double.Parse(rr["sale_price"].ToString()),
                                                        TaxAmount = double.Parse(rr["tax_price"].ToString()),
                                                        Quantity = double.Parse(rr["quantity"].ToString()),
                                                        Comment = rr["comment"].ToString(),
                                                    }).ToList(),
                                       })
                                              .FirstOrDefault();
                deals.Add(fixedDeal);
            }

            return deals;
        }

        private List<OnSpotDeal> GetOnSpotDeals()
        {
            DataTable data = Database.ExecProc("uspApiPOSGetAllOnSpotDeals", null) ?? new DataTable();

            string[] dealNames = (from DataRow x in data.Rows.Cast<DataRow>()
                                  select x["deal_name"].ToString())
                                  .ToArray()
                                  .Distinct()
                                  .ToArray();


            List<OnSpotDeal> deals = new List<OnSpotDeal>();
            foreach (string dealName in dealNames)
            {
                OnSpotDeal onSpotDeal = (from DataRow r in data.Rows.Cast<DataRow>()
                                         where (r["deal_name"].ToString() == dealName)
                                         select new OnSpotDeal
                                         {
                                             UniqueDealId = r["unique_deal_id"].ToString(),
                                             Id = r["deal_id"].ToString(),
                                             CategoryId = r["deal_category_id"].ToString(),
                                             Code = r["deal_code"].ToString(),
                                             Name = r["deal_name"].ToString(),
                                             Price = double.Parse(r["deal_price"].ToString()),
                                             Comment = r["deal_comment"].ToString(),
                                             Quantity = double.Parse(r["deal_quantity"].ToString()),
                                             TaxAmount = double.Parse(r["deal_tax_price"].ToString()),
                                             DealItems = (from DataRow rr in data.Rows.Cast<DataRow>()
                                                          where rr["deal_name"].ToString().Equals(r["deal_name"].ToString())
                                                          select new OnSpotDealItem
                                                          {
                                                              Id = rr["id"].ToString(),
                                                              Code = rr["code"].ToString(),
                                                              CategoryId = rr["category_id"].ToString(),
                                                              Name = rr["item_name"].ToString(),
                                                              Price = double.Parse(rr["sale_price"].ToString()),
                                                              TaxAmount = double.Parse(rr["tax_price"].ToString()),
                                                              Quantity = double.Parse(rr["quantity"].ToString()),
                                                              Comment = rr["comment"].ToString(),
                                                              Choice = double.Parse(rr["choice"].ToString()),
                                                          }).ToList(),
                                         })
                                              .FirstOrDefault();
                deals.Add(onSpotDeal);
            }

            return deals;
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
