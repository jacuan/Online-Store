using DigitalXData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://webapidigitalx.azurewebsites.net/")
        };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult ErrorPage()
        {
            return View();
        }

        public ActionResult LoginPage()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult GetCategoryList()
        {
            HttpResponseMessage response = client.GetAsync("api/CategoryAndProduct/GetCategoryList").Result;

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    object data = response.Content.ReadAsAsync<List<ProductCategory>>().Result;
                    if (data != null)
                    {
                        return View(data);
                    }
                    return View("ErrorPage");
                }
                catch (Exception)
                {
                    return View("ErrorPage");
                }
            }
            return View("ErrorPage");
        }

        [ChildActionOnly]
        public ActionResult SlideShow()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult PopularProducts()
        {
            HttpResponseMessage response = client.GetAsync("api/CategoryAndProduct/GetPopularProductList").Result;

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    object data = response.Content.ReadAsAsync<List<PopularProduct>>().Result;
                    if (data != null)
                    {
                        return View(data);
                    }
                    return View("ErrorPage");
                }
                catch (Exception)
                {
                    return View("ErrorPage");
                }
            }
            return View("ErrorPage");
        }

        [ChildActionOnly]
        public ActionResult ShoppingCart()
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = client.GetAsync("api/PurchaseProduct/CountShoppingCartItems/" + Session["authenticatedUser"].ToString()).Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var number = response.Content.ReadAsAsync<int>().Result;
                        Session["itemNumberInShoppingCart"] = number;
                        return View();
                    }
                    catch (Exception)
                    {
                        Session["itemNumberInShoppingCart"] = 0;
                        return View();
                    }
                }
                Session["itemNumberInShoppingCart"] = 0;
                return View();
                
            }
            Session["itemNumberInShoppingCart"] = 0;
            return View();
        }

        public async Task<ActionResult> OpenAddressForm()
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.GetAsync("api/Customers/GetAddressTypes");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        object types = await response.Content.ReadAsAsync <List<AddressType>>();
                        ViewBag.AddressTypes = types;
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                        Response.Cache.SetNoStore();
                        return View("_AddressForm");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("ErrorPage", "Home");
                    }
                }
                return RedirectToAction("ErrorPage", "Home");

            }
            return RedirectToAction("ErrorPage", "Home");            
        }

    }
}