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
        public IHttpActionResult Get(string key, int tiltId, string orderNo)
        {
            try
            {
                if (!Snippets.Authenticate(key))
                {
                    return Unauthorized();
                }

                if (tiltId < 0)
                {
                    return BadRequest("Invalid tilt id");
                }

                if (tiltId == 0)
                {
                    return BadRequest("Tilt id is missing");
                }

                if ("*".Equals(orderNo))
                {
                    List<Order> newOrders = new List<Order>();
                    DataTable data = new DataTable();
                    data.Columns.Add("data");
                    DataTable orderKeys = Database.GetData($"select order_key from dine_in_order where tiltid = '{ tiltId }' and account_status = 'NOT PAID' and is_delete = 0 order by order_no asc");
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
                    DataTable orderKey = Database.GetData($"select order_key from dine_in_order where tiltid = '{ tiltId }' and order_no = '{ orderNo }' and account_status = 'NOT PAID' and is_delete = 0 order by order_no asc");
                    Order order = DeserializeToOrder(Database.ExecProc("uspApiGetOrders", new string[] { orderKey.Rows[0][0].ToString() }).Rows[0][0].ToString());

                    return Ok(order);
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

                int id = 0;
                DataTable dt = Database.ExecProc("uspApiInsertOrder", new string[] { SerializeToXml(order.Create()) });
                id = Convert.ToInt32(dt.Rows[0][0]);

                if (id >= 0)
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
            System.IO.File.WriteAllText(@"D:\xml.xml", xml);
            return xml;
        }

        private Order DeserializeToOrder(string xml)
        {
            Order order = new Order();
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(Order));
            byte[] vs = Encoding.UTF8.GetBytes(xml);

            using(System.IO.MemoryStream memoryStream = new System.IO.MemoryStream(vs))
            using (XmlReader xmlReader = XmlReader.Create(memoryStream))
            {
                 order = (Order) x.Deserialize(xmlReader);
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

        // Depricated XML Serialzer/De...
        //private Order DecodeXml(string xmlString)
        //{
        //    XmlDocument data = new XmlDocument();
        //    data.LoadXml(xmlString);

        //    XmlDocument values = new XmlDocument();
        //    XmlDocument items = new XmlDocument();
        //    XmlDocument members = new XmlDocument();
        //    Order newOrder = new Order();

        //    XmlNode valueNode = data.SelectSingleNode("data/values");
        //    newOrder.covers = Convert.ToInt32(valueNode.SelectSingleNode("covers").InnerText);
        //    newOrder.table = valueNode.SelectSingleNode("tableNo").InnerText;
        //    newOrder.waiter = valueNode.SelectSingleNode("waiterNo").InnerText;
        //    newOrder.id = valueNode.SelectSingleNode("orderKey").InnerText;
        //    newOrder.orderNo = valueNode.SelectSingleNode("orderNo").InnerText;
        //    newOrder.customer = valueNode.SelectSingleNode("customer").InnerText;
        //    newOrder.contact = valueNode.SelectSingleNode("contact").InnerText;
        //    newOrder.address = valueNode.SelectSingleNode("address").InnerText;
        //    newOrder.orderType = getOrderType(valueNode.SelectSingleNode("orderType").InnerText);
        //    newOrder.date = valueNode.SelectSingleNode("date").InnerText;
        //    newOrder.time = valueNode.SelectSingleNode("time").InnerText;
        //    newOrder.userId = valueNode.SelectSingleNode("userId").InnerText;
        //    XmlNodeList itemsNodeList = data.SelectNodes("data/items/item");
        //    foreach (XmlNode node in itemsNodeList)
        //    {
        //        newOrder.items.Add(new Item()
        //        {
        //            CategoryId = node.SelectSingleNode("categoryId").InnerText,
        //            Id = node.SelectSingleNode("code").InnerText,
        //            Name = node.SelectSingleNode("name").InnerText,
        //            Quantity = node.SelectSingleNode("quantity").InnerText,
        //            Price = node.SelectSingleNode("amount").InnerText,
        //            TaxAmount = node.SelectSingleNode("taxAmount").InnerText
        //        });
        //    }

        //    return newOrder;
        //}

        //private string CreateOrderXml(Order obj)
        //{
        //    try
        //    {
        //        StringBuilder dataString = new StringBuilder();
        //        StringBuilder valuesString = new StringBuilder();
        //        StringBuilder itemsString = new StringBuilder();
        //        XmlWriterSettings settings = new XmlWriterSettings
        //        {
        //            OmitXmlDeclaration = true,
        //        };
        //        XmlWriter xmlWriter = XmlWriter.Create(valuesString, settings);
        //        xmlWriter.WriteStartElement("values");
        //        xmlWriter.WriteElementString("covers", obj.covers.ToString());
        //        xmlWriter.WriteElementString("userid", obj.userId);
        //        xmlWriter.WriteElementString("tiltid", obj.tiltId);
        //        xmlWriter.WriteElementString("waiterNo", obj.waiter);
        //        xmlWriter.WriteElementString("tableNo", obj.table);
        //        xmlWriter.WriteElementString("orderType", obj.orderType);
        //        xmlWriter.WriteElementString("customer", obj.customer);
        //        xmlWriter.WriteElementString("contact", obj.contact);
        //        xmlWriter.WriteElementString("address", obj.address);
        //        xmlWriter.WriteElementString("amount", Convert.ToString(obj.TotalAmount));
        //        xmlWriter.WriteElementString("orderKey", obj.id);
        //        xmlWriter.WriteElementString("orderDate", obj.date);
        //        xmlWriter.WriteEndElement();
        //        xmlWriter.Flush();
        //        xmlWriter.Close();


        //        xmlWriter = XmlWriter.Create(itemsString, settings);
        //        xmlWriter.WriteStartElement("items");
        //        obj.items.ForEach((e) =>
        //        {
        //            xmlWriter.WriteStartElement("item");
        //            xmlWriter.WriteElementString("code", e.Id);
        //            xmlWriter.WriteElementString("name", e.Name);
        //            xmlWriter.WriteElementString("quantity", e.Quantity);
        //            xmlWriter.WriteElementString("price", e.Price);
        //            xmlWriter.WriteElementString("comment", e.Comment);
        //            xmlWriter.WriteElementString("amount", e.Amount);
        //            xmlWriter.WriteEndElement();
        //        });
        //        xmlWriter.WriteEndElement();
        //        xmlWriter.Flush();

        //        xmlWriter = XmlWriter.Create(dataString, settings);
        //        xmlWriter.WriteStartElement("data");
        //        xmlWriter.WriteElementString("values", valuesString.ToString());
        //        xmlWriter.WriteElementString("items", itemsString.ToString());
        //        xmlWriter.WriteEndElement();
        //        xmlWriter.Flush();
        //        xmlWriter.Close();

        //        return dataString.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.Print(ex.Message);
        //        return "";
        //    }
        //}
    }
}
