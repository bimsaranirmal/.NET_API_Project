using System.Web.Mvc;
using SPC.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Linq;



namespace SPC.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public InventoryController()
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

        // GET: InventoryMvc/SearchDrugs
        public ActionResult SearchDrugs()
        {
            return View();
        }
        public ActionResult StaffDrug()
        {
            return View();
        }

        public ActionResult drugs(int supplierId)
        {
            ViewBag.SupplierId = supplierId;
            return View();
        }

        public ActionResult PharmacyDrug(int pharmacyId)
        {
            ViewBag.PharmacyId = pharmacyId;
            return View();
        }
        // POST: InventoryMvc/SearchDrugs
        [HttpPost]
        public async Task<ActionResult> SearchDrugs(string searchTerm)
        {
            var response = await _httpClient.GetStringAsync("inventory/drugs?searchTerm=" + searchTerm);

            var drugs = JsonConvert.DeserializeObject<List<Drug>>(response);
            return View("DrugList", drugs);
        }

        // Other actions
        public ActionResult UpdateStock()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdateStock(int drugId, int quantity)
        {
            var content = new StringContent(quantity.ToString(), Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("inventory/stock/{drugId}", content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Error updating stock.");
            return View();
        }

        public ActionResult AddDrug()
        {
            return View();
        }

        // POST: InventoryMvc/AddDrug (Sends Data to API)
        [HttpPost]
        public async Task<ActionResult> AddDrug(Drug drug)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is not valid.");
                return View(drug);
            }

            try
            {
                Console.WriteLine($"Drug object: {JsonConvert.SerializeObject(drug)}"); // Log the drug object

                var json = JsonConvert.SerializeObject(drug);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("inventory/add", content);

                Console.WriteLine($"Response Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("SearchDrugs");
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Error Response: {errorMessage}");

                ModelState.AddModelError(string.Empty, "Error adding drug: " + errorMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
                Console.WriteLine($"Exception: {ex}");
            }

            return View(drug);
        }

        public ActionResult DrugList()
        {
            // This will call your API or database service to get the list of drugs
            
            return View("DrugList"); // Pass drugs to the view named "DrugLists"
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDrugs()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("inventory/all");
                var drugs = JsonConvert.DeserializeObject<List<Drug>>(response);
                return View("DrugList", drugs); // Pass the drugs to the DrugList view
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error retrieving drugs: " + ex.Message);
                return View("Error"); // Display an error view if something goes wrong
            }
        }

        public ActionResult SearchByPlant()
        {
            return View();
        }

        // POST: Inventory/SearchByPlant
        [HttpPost]
        public async Task<ActionResult> SearchByPlant(string manufacturingPlantId)
        {
            if (string.IsNullOrEmpty(manufacturingPlantId))
            {
                ModelState.AddModelError(string.Empty, "Manufacturing Plant ID is required");
                return View(new List<Drug>()); // Return empty list instead of null
            }

            try
            {
                var response = await _httpClient.GetStringAsync($"Inventory/plant/{manufacturingPlantId}");
                var drugs = JsonConvert.DeserializeObject<List<Drug>>(response);

                if (drugs != null && drugs.Count > 0)
                {
                    return View(drugs);
                }
                else
                {
                    ViewBag.Message = $"No drugs found for Manufacturing Plant ID: {manufacturingPlantId}";
                    return View(new List<Drug>());
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error calling API: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error retrieving drugs. Please try again later.");
                return View(new List<Drug>());
            }
        }

        [HttpGet]
        public async Task<ActionResult> PlantDrugs(string manufacturingPlantId)
        {
            try
            {
                // Updated to use correct endpoint path
                var response = await _httpClient.GetStringAsync($"Inventory/plant/{manufacturingPlantId}");
                var drugs = JsonConvert.DeserializeObject<List<Drug>>(response);
                return View("DrugList", drugs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling API: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error retrieving drugs. Please try again later.");
                return View("Error");
            }
        }

        // Add this helper method to check API availability
        private async Task<bool> IsApiAvailable()
        {
            try
            {
                var response = await _httpClient.GetAsync("Inventory/all");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }


    }
}