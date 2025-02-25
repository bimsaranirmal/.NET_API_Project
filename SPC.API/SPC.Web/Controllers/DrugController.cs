using SPC.Web.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;
using System;

namespace SPC.Web.Controllers
{
    public class DrugController : Controller
    {
        private readonly HttpClient _httpClient;

        public DrugController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
        }

        public async Task<ActionResult> Index()
        {
            var response = await _httpClient.GetStringAsync("drug").ConfigureAwait(false);
            var drugs = JsonConvert.DeserializeObject<List<Drug>>(response);
            return View(drugs);
        }
    }
}
