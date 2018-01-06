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
    public class ProductController : Controller
    {
        HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://webapidigitalx.azurewebsites.net/")
        };

        // Get Product List
        [HttpGet]
        public async Task<ActionResult> GetProductList(int id)
        {
            HttpResponseMessage response = await client.GetAsync("api/CategoryAndProduct/GetProductList/" + id);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    object data = await response.Content.ReadAsAsync<List<ProductDTO>>();
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
        
        [HttpPost]
        public async Task<int> PurchaseProduct(int id, AddToShoppingCart addToShoppingCart)
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.PostAsJsonAsync("api/PurchaseProduct/AddtoShoppingCart/" + id, addToShoppingCart);

                if (response.IsSuccessStatusCode)
                {
                    var addedItem = await response.Content.ReadAsAsync<AddToShoppingCart>();

                    if (addedItem != null)
                    {
                        return 1;
                    }
                    return -1;
                }
                return -1;
            }
            return 0;
        }
    }
}
