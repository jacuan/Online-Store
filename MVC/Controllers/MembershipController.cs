using DigitalXData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace MVC.Controllers
{
    public class MembershipController : Controller
    {
        HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://webapidigitalx.azurewebsites.net/")
        };

        public ActionResult SuccessPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult CreateCustomer()
        {
            return View();
        }

        //Create Customer and Register a New Identity user
        [HttpPost]
        public async Task<ActionResult> CreateCustomer(CustomerDTO customer)
        {
            HttpResponseMessage responseFromApiCustomerController = await client.PostAsJsonAsync("api/Customers", customer);

            var newUserInfor = new Dictionary<string, string>
               {
                   {"Email", customer.UserName},
                   {"Password", customer.Password},
                   {"ConfirmPassword", customer.ConfirmPassword},
               };

            HttpResponseMessage responseFromApiAccountController = await client.PostAsJsonAsync("api/Account/Register", newUserInfor);

            try
            {
                CustomerDTO newCustomer = await responseFromApiCustomerController.Content.ReadAsAsync<CustomerDTO>();

                var repliedStatus = await responseFromApiAccountController.Content.ReadAsAsync<int>();

                if (responseFromApiCustomerController.IsSuccessStatusCode && newCustomer != null && responseFromApiAccountController.IsSuccessStatusCode && repliedStatus == 1)
                {
                    var requestToken = new Dictionary<string, string>
                    {
                         {"grant_type", "password"},
                         {"username", customer.UserName},
                         {"password", customer.Password},
                     };

                    var tokenResponse = await client.PostAsync("Token", new FormUrlEncodedContent(requestToken));
                    var token = await tokenResponse.Content.ReadAsAsync<Token>();

                    if (token.AccessToken != null)
                    {
                        Session["authenticatedUser"] = newCustomer.CustomerId;
                        Session["authenticatedUserFirstName"] = newCustomer.FirstName;
                        Session["authenticatedToken"] = token.AccessToken;

                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                        Response.Cache.SetNoStore();

                        return View("UserCenter", newCustomer);
                    }
                    return RedirectToAction("ErrorPage", "Home");
                }
                return RedirectToAction("ErrorPage", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("ErrorPage", "Home");
            }
        }


        [HttpGet]
        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        public async Task<JsonResult> VerifiyUsernameAvailability(string userName)
        {

            HttpResponseMessage response = await client.GetAsync("api/Customers/CheckUsernameAvailability/" + userName + "/");

            if (response.IsSuccessStatusCode)
            {
                var existingUser = await response.Content.ReadAsAsync<CustomerDTO>();
                if (existingUser == null)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<ActionResult> UserCenter()
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.GetAsync("api/Customers/GetCustomerByID/" + Session["authenticatedUser"].ToString());

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        object data = await response.Content.ReadAsAsync<CustomerDTO>();
                        if (data != null)
                        {
                            return View(data);
                        }
                        return RedirectToAction("ErrorPage", "Home");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("ErrorPage", "Home");
                    }
                }
                return RedirectToAction("ErrorPage", "Home");
            }
            return RedirectToAction("LoginPage", "Home");
        }

        public ActionResult Logout()
        {
            Session.Remove("authenticatedUser");
            Session.Remove("authenticatedUserFirstName");
            Session.Remove("authenticatedToken");
            Session.RemoveAll();
            Session.Abandon();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<int> Login(CustomerDTO customer)
        {
            var requestToken = new Dictionary<string, string>
                    {
                         {"grant_type", "password"},
                         {"username", customer.UserName},
                         {"password", customer.Password},
                     };
            var tokenResponse = await client.PostAsync("Token", new FormUrlEncodedContent(requestToken));
            var token = await tokenResponse.Content.ReadAsAsync<Token>();

            if (token.AccessToken != null)
            {
                HttpResponseMessage response = await client.GetAsync("api/Customers/GetCustomerByUsername/" + customer.UserName + "/");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        CustomerDTO data = await response.Content.ReadAsAsync<CustomerDTO>();
                        if (data != null)
                        {
                            Session["authenticatedUser"] = data.CustomerId;
                            Session["authenticatedUserFirstName"] = data.FirstName;
                            Session["authenticatedToken"] = token.AccessToken;

                            Session["itemNumberInShoppingCart"] = await CountShoppingCartItems(data.CustomerId);

                            return (int)Status.Success;
                        }
                        return (int)Status.Error;
                    }
                    catch (Exception)
                    {
                        return (int)Status.Error;
                    }
                }
                return (int)Status.Error;
            }
            return (int)Status.Error;
        }

        [HttpGet]
        public async Task<int> CountShoppingCartItems(int id)
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.GetAsync("api/PurchaseProduct/CountShoppingCartItems/" + id);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        int count = await response.Content.ReadAsAsync<int>();
                        if (count != 0)
                        {
                            Session["itemNumberInShoppingCart"] = count;
                            return count;
                        }
                        Session["itemNumberInShoppingCart"] = 0;
                        return 0;
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                return 0;
            }
            return 0;
        }


        [HttpPost]
        public async Task<ActionResult> CreateAddress(int id, CustomerAddress customerAddress)
       {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.PostAsJsonAsync("api/Customers/CreateAddress/" + id, customerAddress);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var address = await response.Content.ReadAsAsync<CustomerAddress>();
                        if (address != null)
                        {
                            Response.Cache.SetCacheability(HttpCacheability.NoCache);
                            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                            Response.Cache.SetNoStore();
                            return Json(address, JsonRequestBehavior.AllowGet);
                        }
                        Response.StatusCode = 400;
                        return Json(new { success = false });
                    }
                    catch (Exception)
                    {
                        Response.StatusCode = 400;
                        return Json(new { success = false });
                    }
                }
                Response.StatusCode = 400;
                return Json(new { success = false });
            }
            Response.StatusCode = 400;
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<JsonResult> GetAddressList(int id)
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.GetAsync("api/Customers/GetAddressList/" + id);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var addresses = await response.Content.ReadAsAsync <List<CustomerAddress>>();
                        if (addresses != null)
                        {
                            return Json(addresses, JsonRequestBehavior.AllowGet);
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
            return null;
        }
    }
}
