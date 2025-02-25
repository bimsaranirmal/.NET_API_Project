using System.Web.Mvc;
using SPC.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Net;
using System.Collections.Generic;
using SPC.WEBs.models;
using System.Linq;

namespace SPC.Web.Controllers
{
    public class SupplierDrugOrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public SupplierDrugOrderController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7210/api/");
        }

        // Display Orders List View
        public ActionResult OrderList()
        {
            return View("OrderList");
        }

        public ActionResult ViewOrders(int supplierId)
        {
            ViewBag.SupplierId = supplierId;
            return View("ViewOrders");
        }

        // Fetch all orders from API
        public async Task<ActionResult> GetAllOrders()
        {
            try
            {
                var response = await _httpClient.GetAsync("SupplierDrugOrder");
                if (response.IsSuccessStatusCode)
                {
                    var orders = JsonConvert.DeserializeObject<List<SupplierDrugOrder>>(await response.Content.ReadAsStringAsync());
                    return View("OrderList", orders);
                }
                return View("Error", new HandleErrorInfo(new Exception("Error retrieving orders."), "SupplierDrugOrder", "GetAllOrders"));
            }
            catch (HttpRequestException)
            {
                return View("Error", new HandleErrorInfo(new Exception("Error connecting to API."), "SupplierDrugOrder", "GetAllOrders"));
            }
        }

        // Fetch orders by SupplierId
        public async Task<ActionResult> GetOrdersBySupplierId(int supplierId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SupplierDrugOrder/supplier/{supplierId}");
                if (response.IsSuccessStatusCode)
                {
                    var orders = JsonConvert.DeserializeObject<List<SupplierDrugOrder>>(await response.Content.ReadAsStringAsync());
                    return View("OrderList", orders);
                }
                return View("Error", new HandleErrorInfo(new Exception("No orders found for this supplier."), "SupplierDrugOrder", "GetOrdersBySupplierId"));
            }
            catch (HttpRequestException)
            {
                return View("Error", new HandleErrorInfo(new Exception("Error connecting to API."), "SupplierDrugOrder", "GetOrdersBySupplierId"));
            }
        }

        // Display Order Details
        public async Task<ActionResult> GetOrderById(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"SupplierDrugOrder/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var order = JsonConvert.DeserializeObject<SupplierDrugOrder>(await response.Content.ReadAsStringAsync());
                    return View("OrderDetails", order);
                }
                return View("Error", new HandleErrorInfo(new Exception("Order not found."), "SupplierDrugOrder", "GetOrderById"));
            }
            catch (HttpRequestException)
            {
                return View("Error", new HandleErrorInfo(new Exception("Error retrieving order."), "SupplierDrugOrder", "GetOrderById"));
            }
        }

        // Display Order Creation Form
        public ActionResult PlaceOrder()
        {
            return View();
        }

        // Create a New Order
        [HttpPost]
        public async Task<ActionResult> PlaceOrder(SupplierDrugOrder order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            try
            {
                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("SupplierDrugOrder", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllOrders");
                }
                ModelState.AddModelError("", "Error placing order.");
                return View(order);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Error connecting to API.");
                return View(order);
            }
        }

        // Update an Order
        [HttpPost]
        public async Task<ActionResult> UpdateOrder(int id, SupplierDrugOrder order)
        {
            if (id != order.OrderId)
            {
                return View("Error", new HandleErrorInfo(new Exception("Order ID mismatch."), "SupplierDrugOrder", "UpdateOrder"));
            }

            try
            {
                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"SupplierDrugOrder/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllOrders");
                }
                ModelState.AddModelError("", "Error updating order.");
                return View(order);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Error connecting to API.");
                return View(order);
            }
        }

        // Delete an Order
        [HttpPost]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"SupplierDrugOrder/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("GetAllOrders");
                }
                ModelState.AddModelError("", "Error deleting order.");
                return RedirectToAction("GetAllOrders");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Error connecting to API.");
                return RedirectToAction("GetAllOrders");
            }
        }
    }
}
