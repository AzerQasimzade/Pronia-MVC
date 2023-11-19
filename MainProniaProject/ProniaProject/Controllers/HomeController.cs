using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.ViewModels;

namespace ProniaProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Slide> slides=_context.Slides.OrderBy(s=>s.Order).Take(2).ToList();    
            List<Product>products=_context.Products
                .Include(x => x.ProductImages)
                .ToList();
            List<Product>latestproducts=_context.Products.OrderByDescending(s=>s.Id).Take(8).ToList();
            HomeVM home = new HomeVM
            {
                Slides = slides,
                Products = products,
                LatestProducts = latestproducts,
            };

            return View(home);



        }
        public IActionResult About()
        {
            return View();
        }
    }
}
