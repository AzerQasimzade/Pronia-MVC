using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.Areas.ProniaAdmin.ViewModels;
using ProniaProject.DAL;
using ProniaProject.Models;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products=await _context.Products
                .Include(x=>x.Category)
                .Include(x=>x.ProductImages.Where(pi=>pi.IsPrimary==true))
                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result2 = await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId);
            if (!result2)
            {
                ModelState.AddModelError("CategoryId", "We have not so Product with this Id");
                return View();
            }
            Product product = new Product
            {
                Name = productVM.Name,
                CategoryId = (int)productVM.CategoryId,
                SKU = productVM.SKU,
                Description = productVM.Description,
                Price = productVM.Price
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Details(int id)
        {
            Product product = await _context.Products
                .Include(x=>x.Category)
                .Include(x=>x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            return View(product);
        }


    }
}
