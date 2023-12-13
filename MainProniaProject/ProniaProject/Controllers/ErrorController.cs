using Microsoft.AspNetCore.Mvc;

namespace ProniaProject.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(string error="Heleki xeta bash verdi")
        {
            return View(model:error);
        }
    }
}
