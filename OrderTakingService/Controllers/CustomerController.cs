using OrderTakingService.Lib;
using OrderTakingService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;

namespace OrderTakingService.Controllers
{
    public class CustomerController : ApiController
    {
        public IHttpActionResult Get(string key, string contact)
        {
            try
            {
                if (string.IsNullOrEmpty(contact))
                {
                    return BadRequest("Contact is missing");
                }
                
                if(!Snippets.Authenticate(key))
                {
                    return Unauthorized();
                }

                DataTable dt = Database.ExecProc("uspApiGetCustomer", new string[] { contact });
                List<Customer> customers = (from DataRow dr in dt.Rows.Cast<DataRow>()
                                            select new Customer
                                            {
                                                Id = dr["id"].ToString(),
                                                Name = dr["name"].ToString(),
                                                Contact = dr["contact"].ToString(),
                                                Address = dr["address"].ToString()
                                            }).ToList();
                return Ok(customers);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        public IHttpActionResult Post(string key, Customer customer)
        {
            try
            {
                if (customer == null) return BadRequest("Customer is missing");
                if (!Snippets.Authenticate(key)) return Unauthorized();

                int id = Database.GetIntegerData($"IF(NOT EXISTS(SELECT id FROM CUSTOMER WHERE CellNo = '{ customer.Contact }')) BEGIN INSERT INTO CUSTOMER(Customer, CellNo, Address) VALUES('{ customer.Name }','{ customer.Contact }','{ customer.Address }'); SELECT @@IDENTITY END ELSE BEGIN SELECT 0 END");
                if (id >= 1)
                {
                    DataTable dt = Database.GetData($"SELECT id [id], Customer_name [name], Cell_No [contact], [address] FROM Customerpos WHERE id = {id}") ?? new DataTable(); ;
                    if(dt.Rows.Count >= 1)
                    {
                        Customer newCustomers = (from DataRow dr in dt.Rows.Cast<DataRow>()
                                                    select new Customer
                                                    {
                                                        Id = dr["id"].ToString(),
                                                        Name = dr["name"].ToString(),
                                                        Contact = dr["contact"].ToString(),
                                                        Address = dr["address"].ToString()
                                                    }).ToList().FirstOrDefault();

                        return Created(id.ToString(), newCustomers);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    throw new Exception("Request failed(-1)");
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
