using System.Web.Mvc;
using SPC.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Net;
using System.Collections.Generic;

namespace SPC.Web.Controllers
{
    // Changed the route to match the URL being accessed
    [RoutePrefix("Tenders")]
    public class TendersController : Controller  // Changed class name to match the URL
    {
        private readonly HttpClient _httpClient;

        public TendersController()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
            };
            _httpClient = new HttpClient(handler);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _httpClient.BaseAddress = new Uri("https://localhost:7210/api/tender/");
        }

        // GET: Tenders/Submit
        [HttpGet]
        [Route("Submit")]
        public ActionResult Submit(int supplierId)
        {
            ViewBag.SupplierId = supplierId;
            return View();
        }

        // POST: Tenders/SubmitTender
        [HttpPost]
        [Route("SubmitTender")]
        public async Task<ActionResult> SubmitTender(Tender tender)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(tender);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("submit", content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while submitting the tender.");
                }
            }
            return View(tender);
        }

        public ActionResult ViewAllTenders()
        {
            return View();
        }
        // View all tenders or search based on the search term
        public async Task<ActionResult> GetAllTenders(string searchTerm = "")
        {
            try
            {
                string apiUrl = string.IsNullOrEmpty(searchTerm) ? "" : $"search?searchTerm={searchTerm}";
                var response = await _httpClient.GetAsync(apiUrl).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var tenders = JsonConvert.DeserializeObject<List<Tender>>(jsonResponse);
                    return View(tenders);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while fetching tenders.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View(new List<Tender>());
        }

        // ✅ New Method: Get tenders by Supplier ID
        [HttpGet]
        [Route("SupplierTenders/{supplierId}")]
        public async Task<ActionResult> GetTendersBySupplierId(int supplierId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"supplier/{supplierId}").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var tenders = JsonConvert.DeserializeObject<List<Tender>>(jsonResponse);

                    ViewBag.SupplierId = supplierId; // Ensure Supplier ID is passed to the view
                    return View("GetTendersBySupplierId",tenders);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, $"No tenders found for Supplier ID {supplierId}.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            ViewBag.SupplierId = supplierId; // Pass the supplier ID even if there are no tenders
            return View(new List<Tender>());
        }


        // Approve a tender
        [HttpPost]
        public async Task<ActionResult> Approve(int id)
        {
            var response = await _httpClient.PutAsync($"approve/{id}", null).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, action = "Approve", id = id });
            }
            else
            {
                return Json(new { success = false, message = "Error occurred while approving the tender." });
            }
        }

        // Reject a tender
        [HttpPost]
        public async Task<ActionResult> Reject(int id)
        {
            var response = await _httpClient.PutAsync($"reject/{id}", null).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, action = "Reject", id = id });
            }
            else
            {
                return Json(new { success = false, message = "Error occurred while rejecting the tender." });
            }
        }

        // Delete a tender
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _httpClient.PutAsync($"done/{id}", null).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, action = "Delete", id = id });
            }
            else
            {
                return Json(new { success = false, message = "Error occurred while marking tender as done." });
            }
        }

    }
}