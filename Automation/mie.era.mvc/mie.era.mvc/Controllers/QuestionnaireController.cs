using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace mie.era.mvc.Controllers
{
    [Authorize]
    public class QuestionnaireController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
