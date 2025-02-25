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

namespace SPC.Web.Controllers
{
    public class SupplierController : Controller
    {
        private readonly HttpClient _httpClient;

        public SupplierController()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            _httpClient = new HttpClient(handler);

            // Forcing TLS 1.2 (you can try other versions if necessary)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _httpClient.BaseAddress = new Uri("https://localhost:7210/api/");
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("supplier/register", content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("SupplierLogin", "Supplier");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while registering the supplier.");
                }
            }

            return View(supplier);
        }

        // View all suppliers or search based on the search term
        public async Task<ActionResult> ViewAll(string searchTerm = "")
        {
            try
            {
                // Check if the search term is provided, if so, perform a search
                string apiUrl = string.IsNullOrEmpty(searchTerm) ? "supplier/all" : $"supplier/search?searchTerm={searchTerm}";
                var response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var suppliers = JsonConvert.DeserializeObject<List<Supplier>>(jsonResponse);
                    return View(suppliers);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while fetching suppliers.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View(new List<Supplier>());
        }



        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"supplier/delete/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ViewAll");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error occurred while deleting the supplier.");
                return RedirectToAction("ViewAll");
            }
        }

        [HttpGet]
        public ActionResult SupplierLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View("SupplierLogin");
            }

            try
            {
                var credentials = new { email, password };
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("supplier/login", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseSupplier>(responseBody);

                    if (loginResponse?.Supplier == null || loginResponse.Supplier.Id == 0)
                    {
                        throw new Exception("Invalid response from server. Supplier ID missing.");
                    }

                    // Store supplier info in session
                    // From your SupplierController.LoginAsync method
                    Session["SupplierId"] = loginResponse.Supplier.Id;
                    Session["SupplierName"] = loginResponse.Supplier.Name;

                    return RedirectToAction("SupplierNavigation");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View("SupplierLogin");
        }

        // Supplier Navigation Action
        
        public ActionResult SupplierNavigation()
        {
            if (Session["SupplierId"] != null)
            {
                ViewBag.SupplierId = Session["SupplierId"];
                TempData["SupplierId"] = Session["SupplierId"];
            }
            else
            {
                ViewBag.PharmacyId = "Not logged in";
            }

            return View();
        }


        // ✅ Approve Supplier
        [HttpPost]
        public async Task<ActionResult> Approve(int id)
        {
            var response = await _httpClient.PostAsync($"supplier/approve/{id}", null).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ViewAll");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error occurred while approving the supplier.");
                return RedirectToAction("ViewAll");
            }
        }

        // ❌ Reject Supplier
        [HttpPost]
        public async Task<ActionResult> Reject(int id)
        {
            var response = await _httpClient.PostAsync($"supplier/reject/{id}", null).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ViewAll");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error occurred while rejecting the supplier.");
                return RedirectToAction("ViewAll");
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();  // Clear all session data
            Session.Abandon(); // Abandon the session
            return RedirectToAction("SupplierLogin");
        }

    }
}
