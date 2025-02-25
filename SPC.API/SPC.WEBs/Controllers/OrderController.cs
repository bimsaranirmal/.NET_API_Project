using System.Web.Mvc;
using SPC.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;
using System;
using System.Net;

namespace SPC.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7210/api/");
        }
        public ActionResult OrderList()
        {
            return View();
        }
        public async Task<ActionResult> GetAllOrders()
        {
            try
            {
                // Send GET request to the API to fetch all orders
                var response = await _httpClient.GetAsync("order").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response into a list of orders
                    var orders = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());

                    // Return the view with the list of orders
                    return View("OrderList", orders);
                }
                else
                {
                    return View("Error", new HandleErrorInfo(new Exception("Error retrieving order list."), "Order", "GetAllOrders"));
                }
            }
            catch (HttpRequestException)
            {
                // Handle errors (e.g., API not reachable)
                return View("Error", new HandleErrorInfo(new Exception("Error retrieving order list."), "Order", "GetAllOrders"));
            }
        }

        public ActionResult PlaceOrder(int pharmacyId)
        {
            // Use the pharmacyId in your logic
            ViewBag.PharmacyId = pharmacyId;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> PlaceOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            var pharmacyResponse = await _httpClient.GetAsync($"pharmacy/{order.PharmacyId}");
            if (!pharmacyResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("PharmacyId", "Invalid Pharmacy ID. Please check the ID and try again.");
                return View(order);
            }

            try
            {
                // Ensure Status is included
                order.Status = "Pending";

                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("order", content).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("OrderDetails");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error placing order.");
                    return View(order);
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error placing order.");
                return View(order);
            }
        }

        // Action to get order by ID
        public async Task<ActionResult> GetOrderById(int orderId)
        {
            try
            {
                // Send GET request to fetch a specific order by its ID
                var response = await _httpClient.GetAsync($"order/{orderId}").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response into an order object
                    var order = JsonConvert.DeserializeObject<Order>(await response.Content.ReadAsStringAsync());

                    // Return the order details view with the fetched order
                    return View("OrderDetails", order);
                }
                else
                {
                    // Handle case where the order is not found
                    return View("Error", new HandleErrorInfo(new Exception("Order not found."), "Order", "GetOrderById"));
                }
            }
            catch (HttpRequestException)
            {
                // Handle network or API unreachable errors
                return View("Error", new HandleErrorInfo(new Exception("Error retrieving order."), "Order", "GetOrderById"));
            }
        }

        // ✅ Approve Order
        [HttpPost]
        public async Task<ActionResult> ApproveOrder(int orderId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"order/approve/{orderId}", null).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllOrders");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error approving order.");
                    return RedirectToAction("GetAllOrders");
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error approving order.");
                return RedirectToAction("GetAllOrders");
            }
        }

        // ❌ Reject Order
        [HttpPost]
        public async Task<ActionResult> RejectOrder(int orderId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"order/reject/{orderId}", null).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllOrders");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error rejecting order.");
                    return RedirectToAction("GetAllOrders");
                }
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error rejecting order.");
                return RedirectToAction("GetAllOrders");
            }
        }

        public async Task<ActionResult> OrdersDetails(int pharmacyId)
        {
            try
            {
                // Verify if the logged-in pharmacy matches the requested pharmacyId
                // This ensures a pharmacy can only view their own orders
                int loggedInPharmacyId = pharmacyId; // Use this as the filter

                ViewBag.PharmacyId = loggedInPharmacyId; // Pass to view
                var response = await _httpClient.GetAsync($"order?pharmacyId={loggedInPharmacyId}");

                if (!response.IsSuccessStatusCode)
                {
                    return View("Error", new HandleErrorInfo(new Exception("Error retrieving order list."), "Order", "OrdersDetails"));
                }

                var orders = JsonConvert.DeserializeObject<List<Order>>(await response.Content.ReadAsStringAsync());
                return View(orders);
            }
            catch (Exception ex)
            {
                return View("Error", new HandleErrorInfo(ex, "Order", "OrdersDetails"));
            }
        }






    }
}
