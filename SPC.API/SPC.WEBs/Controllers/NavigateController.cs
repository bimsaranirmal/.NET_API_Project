using System.Web.Mvc;
// Only this is needed for ASP.NET Core MVC

namespace SPC.WEBs.Controllers
{
    public class NavigateController : Controller
    {
        public ActionResult StaffNavigation()
        {
            return View();  // This will look for Views/Navigate/StaffNavigation.cshtml
        }
        public ActionResult StaffUserNavigation()
        {
            return View();  // This will look for Views/Navigate/StaffNavigation.cshtml
        }

        public ActionResult PharmacyNavigation(int pharmacyId)
        {
            ViewBag.PharmacyId = pharmacyId;
            return View();
        }

        public ActionResult SupplierNavigation(int supplierId)
        {
            ViewBag.SupplierId = supplierId;
            return View();  // This will look for Views/Navigate/StaffNavigation.cshtml
        }
    }
}
