using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Http;
using System.Xml;

namespace OrderTakingService.Controllers
{
    public class OrderController : ApiController
    {
        public IHttpActionResult Get(string key, string deviceId, string orderNo)
        {
            try
            {
                if (!Snippets.Authenticate(key))
                {
                    return Unauthorized();
                }

                if (deviceId.Equals(string.Empty))
                {
                    return BadRequest("Invalid device id");
                }

                if ("*".Equals(orderNo))
                {
                    List<Order> newOrders = new List<Order>();
                    DataTable orderKeys = Database.ExecProc("uspApiGetOrderIds", new string[] { deviceId, string.Empty });
                    for (int i = 0; i < orderKeys.Rows.Count; i++)
                    {
                        try
                        {
                            newOrders.Add(DeserializeToOrder(Database.ExecProc("uspApiGetOrders", new string[] { orderKeys.Rows[i][0].ToString() }).Rows[0][0].ToString()));
                        }
                        catch { }
                    }

                    return Ok(newOrders);
                }
                else if (int.TryParse(orderNo, out int number))
                {
                    DataTable orderKey = Database.ExecProc("uspApiGetOrderIds", new string[] { deviceId, number.ToString() }) ?? new DataTable();
                    if (orderKey.Rows.Count >= 1)
                    {
                        Order order = DeserializeToOrder(Database.ExecProc("uspApiGetOrders", new string[] { orderKey.Rows[0][0].ToString() }).Rows[0][0].ToString());
                        return Ok(order);
                    }
                    else
                    {
                        return NotFound();
                    }

                }
                else
                {
                    return BadRequest("Invalid order number");
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public IHttpActionResult Post(string key, Order order)
        {
            try
            {
                if (!Snippets.Authenticate(key))
                {
                    return Unauthorized();
                }

                if (order == null)
                {
                    return BadRequest("Order is missing");
                }

                string id = "";
                DataTable dt = Database.ExecProc("uspApiInsertOrder", new string[] { SerializeToXml(order.Create()) });
                id = dt.Rows[0][0].ToString();

                if (!new List<string> { "", "0", "-1", "-2" }.Contains(id))
                {
                    return Created(id.ToString(), order);
                }
                else
                {
                    throw new Exception("Order cannot be created");
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public IHttpActionResult Put(string key, Order order)
        {
            try
            {
                if (!Snippets.Authenticate(key))
                {
                    return Unauthorized();
                }

                if (order == null)
                {
                    return BadRequest("Order is missing");
                }

                order.Date = DateTime.Now.ToString("yyyy-MM-dd");
                order.Time = DateTime.Now.ToString("HH:mm");
                int id = -1;

                DataTable dt = Database.ExecProc("uspApiUpdateOrder", new string[] { SerializeToXml(order.Create()) });
                id = Convert.ToInt32(dt.Rows[0][0]);

                if (id >= 0)
                {
                    return Created(id.ToString(), order);
                }
                else
                {
                    throw new Exception("Order cannot be updated");
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public IHttpActionResult Delete(string key, int orderkey)
        {
            if (!Snippets.Authenticate(key))
            {
                return Unauthorized();
            }

            return InternalServerError(new NotImplementedException(Snippets.RequestNotSupported));
        }

        private string getOrderType(string orderType)
        {
            if (orderType.Equals("DINE IN"))
            {
                return "1";
            }
            else if (orderType.Equals("TAKE AWAY"))
            {
                return "2";
            }
            else if (orderType.Equals("DELIVERY"))
            {
                return "3";
            }
            else
            {
                return "0";
            }
        }

        private string SerializeToXml(Order order)
        {
            string xml = string.Empty;

            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(order.GetType());

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, new XmlWriterSettings { Indent = false, OmitXmlDeclaration = true, Encoding = Encoding.UTF8 }))
            {
                x.Serialize(xmlWriter, order);
                xml = Encoding.UTF8.GetString(memoryStream.GetBuffer());
            }
            System.IO.File.WriteAllText(@"D:\order.xml", xml);
            return xml;
        }

        private Order DeserializeToOrder(string xml)
        {
            Order order = new Order();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Order));
            byte[] vs = Encoding.UTF8.GetBytes(xml);

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(vs))
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                order = (Order)x.Deserialize(xmlReader);
            }

            switch (order.OrderType)
            {
                case "DINE IN":
                    order.OrderType = "1";
                    break;
                case "TAKE AWAY":
                    order.OrderType = "2";
                    break;
                case "DELIVERY":
                    order.OrderType = "3";
                    break;
                default:
                    break;
            }
            return order.Create();
        }
    }
}
