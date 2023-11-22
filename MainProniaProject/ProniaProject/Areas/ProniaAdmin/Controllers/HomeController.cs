using Microsoft.AspNetCore.Mvc;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
