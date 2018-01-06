using DigitalXData;
using MVC.OrderServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class OrderController : Controller
    {
        OrderServiceReference.OrderInformationServiceClient orderProxy = new OrderServiceReference.OrderInformationServiceClient();

        HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://webapidigitalx.azurewebsites.net/")
        };

        [HttpGet]
        public async Task<ActionResult> GetOrderInformation()
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                string customerId = Session["authenticatedUser"].ToString();

                try
                {
                    var orderedItems = await orderProxy.GetOrderListAsync(Convert.ToInt32(customerId));
                    return View(orderedItems);
                }
                catch(FaultException<RequestFailed> e)
                {
                    FaultException<RequestFailed> requestFailed = e as FaultException<RequestFailed>;
                    string errorMessage = "Error " + requestFailed.Detail.ErrorCode + ": " + requestFailed.Detail.Message;
                    ViewBag.errorMessage = errorMessage;
                    return View();
                }
            }
            return RedirectToAction("LoginPage", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> DisplayShoppingCart()
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                string customerId = Session["authenticatedUser"].ToString();

                try
                {
                    var shoppingCartInformation = await orderProxy.GetShoppingCartInformationAsync(Convert.ToInt32(customerId));
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
                    Response.Cache.SetNoStore();
                    return View(shoppingCartInformation);
                }
                catch (FaultException<RequestFailed> e)
                {
                    FaultException<RequestFailed> requestFailed = e as FaultException<RequestFailed>;
                    string errorMessage = "Error " + requestFailed.Detail.ErrorCode + ": " + requestFailed.Detail.Message;
                    ViewBag.errorMessage = errorMessage;
                    return View();
                }
            }
            return RedirectToAction("LoginPage", "Home");
        }

        [HttpPost]
        public async Task<int> AdjustQuantity(int id, AdjustQuantity quantity)
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.PostAsJsonAsync("api/PurchaseProduct/AdjustQuantity/" + id, quantity);

                if (response.IsSuccessStatusCode)
                {
                    var adjustedQuantity = await response.Content.ReadAsAsync<AdjustQuantity>();

                    if (adjustedQuantity != null)
                    {
                        return 1;
                    }
                    return -1;
                }
                return -1;
            }
            return 0;
        }

        [HttpGet]
        public async Task<int> DeleteItem(int id)
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.GetAsync("api/PurchaseProduct/DeleteItem/" + id);

                if (response.IsSuccessStatusCode)
                {
                    var adjustedQuantity = await response.Content.ReadAsAsync<int>();

                    if (adjustedQuantity == 1)
                    {
                        return 1;
                    }
                    return -1;
                }
                return -1;
            }
            return 0;
        }

        public ActionResult ThankYouPage()
        {
            return View();
        }


        [HttpGet]
        public async Task<JsonResult> DeductStockConditionaly(int id)
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage response = await client.GetAsync("api/PurchaseProduct/VerifyStock/" + id);

                if (response.IsSuccessStatusCode)
                {
                    List<VerifiyStock> lists = await response.Content.ReadAsAsync<List<VerifiyStock>>();

                    if (lists != null)
                    {
                        List<VerifiyStock> backorderItems = (from b in lists
                                              where b.OrderedQuantity > b.AvailableStock
                                              select b).ToList();

                        if (backorderItems.Count() > 0)
                        {
                            return Json(backorderItems, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                            HttpResponseMessage deductStock = await client.GetAsync("api/PurchaseProduct/DeductStockConditionaly/" + id);

                            if (deductStock.IsSuccessStatusCode)
                            {
                                int result = await deductStock.Content.ReadAsAsync<int>();
                                if (result == 1)
                                {
                                    return Json(1, JsonRequestBehavior.AllowGet);
                                }
                                return Json(0, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                return Json(0, JsonRequestBehavior.AllowGet);
                            }    
                        }  
                    }
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public async Task<JsonResult> DeductStockWithBackorder(int id)
        {
            if (Session["authenticatedUser"] != null && Session["authenticatedToken"] != null)
            {
                client.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["authenticatedToken"].ToString());

                HttpResponseMessage deductStock = await client.GetAsync("api/PurchaseProduct/DeductStockWithBackorder/" + id);

                if (deductStock.IsSuccessStatusCode)
                {
                    int result = await deductStock.Content.ReadAsAsync<int>();
                    if (result == 1)
                    {
                        return Json(1, JsonRequestBehavior.AllowGet);
                    }
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }
    }
}
