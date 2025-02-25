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
    public class PharmacyController : Controller
    {
        private readonly HttpClient _httpClient;

        public PharmacyController()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            _httpClient = new HttpClient(handler);

            // Forcing TLS 1.2 (you can try other versions if necessary)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            _httpClient.BaseAddress = new Uri("https://localhost:7210/api/");  // Adjust the API URL if needed
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(Pharmacy pharmacy)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(pharmacy);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("pharmacy/register", content).ConfigureAwait(false);  // Adjust the API endpoint if needed

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("PharmacyLogin", "Pharmacy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while registering the pharmacy.");
                }
            }

            return View(pharmacy);
        }

        public async Task<ActionResult> Index(string searchTerm = "")
        {
            try
            {
                string apiEndpoint = string.IsNullOrEmpty(searchTerm)
                    ? "pharmacy/all"
                    : $"pharmacy/search?query={searchTerm}";

                var response = await _httpClient.GetAsync(apiEndpoint).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var pharmacies = JsonConvert.DeserializeObject<List<Pharmacy>>(jsonResponse);
                    return View(pharmacies);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while fetching pharmacies.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View(new List<Pharmacy>());
        }


        [HttpGet]
        public async Task<ActionResult> Search(string searchTerm)
        {
            return await Index(searchTerm);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"pharmacy/delete/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");  // Redirect to the list of pharmacies
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error occurred while deleting the pharmacy.");
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult PharmacyLogin()
        {
            return View();
        }

        public async Task<ActionResult> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View("PharmacyLogin");
            }

            try
            {
                var credentials = new { email, password };
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("pharmacy/login", content).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseBody);

                    if (loginResponse?.Pharmacy == null || loginResponse.Pharmacy.Id == 0)
                    {
                        throw new Exception("Invalid response from server. Pharmacy ID missing.");
                    }

                    Session["PharmacyId"] = loginResponse.Pharmacy.Id;
                    Session["PharmacyName"] = loginResponse.Pharmacy.PharmacyName;

                    return RedirectToAction("PharmacyNavigation");
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

            return View("PharmacyLogin");
        }

        // Pharmacy Navigation Action
        public ActionResult PharmacyNavigation()
        {
            if (Session["PharmacyId"] != null)
            {
                ViewBag.PharmacyId = Session["PharmacyId"];
                TempData["PharmacyId"] = Session["PharmacyId"];
            }
            else
            {
                ViewBag.PharmacyId = "Not logged in";
            }

            return View();
        }



        public async Task<ActionResult> GetPharmacyById(int pharmacyId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"pharmacy/{pharmacyId}").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var pharmacy = JsonConvert.DeserializeObject<Pharmacy>(jsonResponse);

                    return View(pharmacy);  // Display pharmacy details in the view
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Pharmacy not found.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View(new Pharmacy());  // Return an empty pharmacy model in case of failure
        }

        public async Task<ActionResult> AllPharmacies(string searchTerm = "")
        {
            try
            {
                string apiEndpoint = string.IsNullOrEmpty(searchTerm)
                    ? "pharmacy/all"
                    : $"pharmacy/search?query={searchTerm}";

                var response = await _httpClient.GetAsync(apiEndpoint).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var pharmacies = JsonConvert.DeserializeObject<List<Pharmacy>>(jsonResponse);
                    return View(pharmacies);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while fetching pharmacies.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View(new List<Pharmacy>());
        }

        public ActionResult Logout()
        {
            Session.Clear();  // Clear all session data
            Session.Abandon(); // Abandon the session
            return RedirectToAction("PharmacyLogin");
        }

        public async Task<ActionResult> staff(string searchTerm = "")
        {
            try
            {
                string apiEndpoint = string.IsNullOrEmpty(searchTerm)
                    ? "pharmacy/all"
                    : $"pharmacy/search?query={searchTerm}";

                var response = await _httpClient.GetAsync(apiEndpoint).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var pharmacies = JsonConvert.DeserializeObject<List<Pharmacy>>(jsonResponse);
                    return View(pharmacies);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while fetching pharmacies.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View(new List<Pharmacy>());
        }

    }
}
