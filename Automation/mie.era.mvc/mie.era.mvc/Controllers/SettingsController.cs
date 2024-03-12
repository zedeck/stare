using Microsoft.AspNetCore.Mvc;

namespace mie.era.mvc.Controllers
{
    public class SettingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
