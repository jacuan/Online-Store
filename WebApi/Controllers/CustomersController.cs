using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DigitalXData;
using WebApi.Models;
using System.Web.UI;

namespace WebApi.Controllers
{
    [Authorize]
    public class CustomersController : ApiController
    {
        DigitalXDB_JackyWebEntities db = new DigitalXDB_JackyWebEntities();

        // GET customer by ID
        [HttpGet]
        [Route("api/Customers/GetCustomerByID/{id}")]
        public async Task<CustomerDTO> GetCustomerByID(int id)
        {
            try
            {
                Customer customer = await db.Customers.SingleAsync(a => a.CustomerID == id);
                if (customer != null)
                {
                    return new CustomerDTO()
                    {
                        CustomerId = customer.CustomerID,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        UserName = customer.UserName,
                    };
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Login
        [HttpGet]
        [AllowAnonymous]
        [Route("api/Customers/GetCustomerByUsername/{userName}")]
        public async Task<CustomerDTO> GetCustomerByUsername(string userName)
        {
            try
            {
                Customer customer = await db.Customers.SingleAsync(a => a.UserName == userName);
                if (customer != null)
                {
                    return new CustomerDTO()
                    {
                        CustomerId = customer.CustomerID,
                        FirstName = customer.FirstName,
                        LastName = customer.LastName,
                        UserName = customer.UserName,
                    };
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // Create a customer
        [HttpPost]
        [AllowAnonymous]
        [ResponseType(typeof(CustomerDTO))]
        public async Task<IHttpActionResult> CreateCustomer(CustomerDTO customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Customer newCustomer = new Customer();
            newCustomer.FirstName = customer.FirstName;
            newCustomer.LastName = customer.LastName;
            newCustomer.Password = customer.Password;
            newCustomer.UserName = customer.UserName;
            db.Customers.Add(newCustomer);
            await db.SaveChangesAsync();

            customer.CustomerId = newCustomer.CustomerID;
            return Ok(customer);
        }

        // check username availability
        [HttpGet]
        [AllowAnonymous]
        [Route("api/Customers/CheckUsernameAvailability/{userName}")]
        [System.Web.Mvc.OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public async Task<CustomerDTO> CheckUsernameAvailability(string userName)
        {
            var context = new ApplicationDbContext();

            var user = await (from u in context.Users
                              where u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                              select u).FirstOrDefaultAsync();

            var customer = await (from c in db.Customers
                                  where c.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase)
                                  select c).FirstOrDefaultAsync();

            if (user != null || customer != null)
            {
                CustomerDTO customerDTO = new CustomerDTO();
                customerDTO.UserName = userName;
                return customerDTO;
            }
            else
            {
                return null;
            }
        }


        //Get address types
        [HttpGet]
        [Route("api/Customers/GetAddressTypes")]
        public List<AddressType> GetAddressTypes()
        {
            var types = (from t in db.AddressTypes
                         select t);
            if (types != null)
            {
                return types.ToList();
            }
            return null;
        }


        //Create an address 
        [HttpPost]
        [Route("api/Customers/CreateAddress/{id}")]
        public async Task<CustomerAddress> CreateAddress(int id, CustomerAddress customerAddress)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Address address = new Address();
                    address.AddressType = customerAddress.AddressType;
                    address.Street = customerAddress.Street;
                    address.Suburb = customerAddress.Suburb;
                    address.City = customerAddress.City;
                    address.Country = customerAddress.Country;
                    address.PostalCode = customerAddress.PostalCode;

                    var customer = await(from c in db.Customers
                                         where c.CustomerID == id
                                         select c).FirstOrDefaultAsync();

                    if (customer != null)
                    {
                        try
                        {
                            customer.Addresses.Add(address);
                            await db.SaveChangesAsync();
                            return customerAddress;
                        }
                        catch (Exception)
                        {
                            return null;
                        }
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }


        //Get address list
        [HttpGet]
        [Route("api/Customers/GetAddressList/{id}")]
        public async Task<List<CustomerAddress>> GetAddressList(int id)
        {
            try
            {
                var addresses = (from c in db.Customers
                                 from a in c.Addresses
                                 where c.CustomerID == id && a.PostalCode != "Dummy"
                                 select new CustomerAddress()
                                 {
                                     Street = a.Street,
                                     Suburb = a.Suburb,
                                     City = a.City,
                                     Country = a.Country,
                                     PostalCode = a.PostalCode,
                                     AddressType = a.AddressType,
                                     AddressId = a.AddressID
                                 });

                return await addresses.ToListAsync();
                                
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}