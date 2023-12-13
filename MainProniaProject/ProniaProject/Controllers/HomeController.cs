using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.Services;
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
        public  async Task<IActionResult> Index()
        {
            List<Slide> slides=await _context.Slides.OrderBy(s=>s.Order).Take(2).ToListAsync();    
            List<Product>products=await _context.Products
                .Include(x => x.ProductImages.Where(pi=>pi.IsPrimary!=null))
                .ToListAsync();
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
