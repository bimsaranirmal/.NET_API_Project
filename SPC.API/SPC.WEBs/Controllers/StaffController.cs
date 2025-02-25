using System.Web.Mvc;
using SPC.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Net;
using System.Collections.Generic;
using SPC.Web.Models;
using SPC.WEBs.models;

namespace SPC.Web.Controllers
{
    public class StaffController : Controller
    {
        private readonly HttpClient _httpClient;

        public StaffController()
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

        // Register Staff Action
        public ActionResult StaffRegister()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(Staff staff)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(staff);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("staff/register", content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("StaffLogin", "Staff");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error occurred while registering the staff member.");
                }
            }

            return View(staff);
        }

        public ActionResult StaffManage()
        {
            return View();
        }
        public async Task<ActionResult> ViewAll()
        {
            List<Staff> staffList = new List<Staff>();

            try
            {
                // Assuming you want to fetch staff data, use GET method instead of POST
                var response = await _httpClient.GetAsync("staff/all").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    staffList = JsonConvert.DeserializeObject<List<Staff>>(jsonResponse) ?? new List<Staff>();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error fetching staff members.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View("StaffManage", staffList);  // Ensure it points to the correct view
        }



        // Delete Staff
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"staff/delete/{id}").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("StaffManage");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error occurred while deleting the staff member.");
                return RedirectToAction("StaffManage");
            }
        }

        // Staff Login (GET)
        [HttpGet]
        public ActionResult StaffLogin()
        {
            return View();
        }

        // Staff Login (POST)
        [HttpPost]
        public async Task<ActionResult> LoginAsync(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View("StaffLogin");
            }

            try
            {
                var credentials = new { email, password };
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("staff/login", content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    // Create a class to match the API response structure
                    var loginResponse = JsonConvert.DeserializeObject<StaffLoginResponse>(jsonResponse);

                    if (loginResponse?.Staff != null)
                    {
                        Session["StaffId"] = loginResponse.Staff.Id;
                        Session["StaffName"] = loginResponse.Staff.Name;
                        Session["StaffRole"] = loginResponse.Staff.Role;

                        // Now check the role and redirect
                        if (loginResponse.Staff.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("AdminDashboard");
                        }
                        else if (loginResponse.Staff.Role.Equals("User", StringComparison.OrdinalIgnoreCase))
                        {
                            return RedirectToAction("StaffNavigation");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Invalid role assignment.");
                            return View("StaffLogin");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred: " + ex.Message);
            }

            return View("StaffLogin");
        }

        // Add this class to match the API response structure
        

        public ActionResult Logout()
        {
            Session.Clear();  // Clear all session data
            Session.Abandon(); // Abandon the session
            return RedirectToAction("StaffLogin");
        }

    }
}
