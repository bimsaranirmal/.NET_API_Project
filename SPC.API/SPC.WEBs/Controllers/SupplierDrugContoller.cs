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
    public class SupplierDrugController : Controller
    {
        private readonly HttpClient _httpClient;

        public SupplierDrugController()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7210/api/");
        }

        // GET: SupplierDrug/Add
        public ActionResult Add()
        {
            return View();
        }

        // POST: SupplierDrug/Add
        [HttpPost]
        public async Task<ActionResult> Add(SupplierDrug supplierDrug)
        {
            if (!ModelState.IsValid)
            {
                return View(supplierDrug);
            }

            try
            {
                var json = JsonConvert.SerializeObject(supplierDrug);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("SupplierDrug/add", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Drug added successfully!";
                    return RedirectToAction("Add");
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Error adding supplier drug: " + errorMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
            }

            return View(supplierDrug);
        }


        // GET: SupplierDrug/SearchDrugs
        public ActionResult SearchDrugs()
        {
            return View();
        }

        // POST: SupplierDrug/SearchDrugs
        [HttpPost]
        public async Task<ActionResult> SearchDrugs(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"SupplierDrug/drugSearch?searchTerm={searchTerm}");
                var drugs = JsonConvert.DeserializeObject<List<SupplierDrug>>(response);
                return View("DrugList", drugs);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error searching drugs: " + ex.Message);
                return View();
            }
        }
        public ActionResult SupplierDrug()
        {
            

            return View("SupplierDrug"); // Pass drugs to the view named "DrugLists"
        }



        public ActionResult AllDrugs(int supplierId)
        {
            ViewBag.SupplierId = supplierId;

            return View("AllDrugs"); // Pass drugs to the view named "DrugLists"
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDrugs()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("SupplierDrug/drugs");
                var drugs = JsonConvert.DeserializeObject<List<SupplierDrug>>(response);
                return View("AllDrugs", drugs); // Pass the drugs to the DrugList view
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error retrieving drugs: " + ex.Message);
                return View("Error"); // Display an error view if something goes wrong
            }
        }



        // GET: SupplierDrug/GetBySupplier
        public ActionResult GetBySupplier()
        {
            return View();
        }

        // POST: SupplierDrug/GetBySupplier
        [HttpPost]
        public async Task<ActionResult> GetBySupplier(int supplierId)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"SupplierDrug/supplier/{supplierId}");
                var drugs = JsonConvert.DeserializeObject<List<SupplierDrug>>(response);
                return View("DrugList", drugs);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error retrieving supplier drugs: " + ex.Message);
                return View();
            }
        }


        // GET: SupplierDrug/Update
        public async Task<ActionResult> Update(int id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"SupplierDrug/{id}");
                var drug = JsonConvert.DeserializeObject<SupplierDrug>(response);
                return View(drug);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error retrieving drug: " + ex.Message);
                return RedirectToAction("SearchDrugs");
            }
        }

        // POST: SupplierDrug/Update
        [HttpPost]
        public async Task<ActionResult> Update(SupplierDrug supplierDrug)
        {
            if (!ModelState.IsValid)
            {
                return View(supplierDrug);
            }

            try
            {
                var json = JsonConvert.SerializeObject(supplierDrug);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("SupplierDrug/update", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("SearchDrugs");
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Error updating supplier drug: " + errorMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
            }

            return View(supplierDrug);
        }

        // POST: SupplierDrug/Delete
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"SupplierDrug/delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("SearchDrugs");
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Error deleting supplier drug: " + errorMessage);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex.Message);
            }

            return RedirectToAction("SearchDrugs");
        }
    }
}
