using SPC.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace SPC.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
        }

        public async Task<ActionResult> OrderList()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("order").ConfigureAwait(false);
                var orders = JsonConvert.DeserializeObject<List<Order>>(response);
                return View(orders);
            }
            catch (HttpRequestException)
            {
                // Log the exception here
                return View("Error", new HandleErrorInfo(new Exception("Error retrieving order list."), "Order", "OrderList"));
            }
        }

        public ActionResult PlaceOrder()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<ActionResult> PlaceOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            try
            {
                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("order/place", content).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("OrderList");
                }
                else
                {
                    // Log the response status code and reason here
                    ModelState.AddModelError(string.Empty, "Error placing order.");
                    return View(order);
                }
            }
            catch (HttpRequestException)
            {
                // Log the exception here
                ModelState.AddModelError(string.Empty, "Error placing order.");
                return View(order);
            }
        }
    }
}
