using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]

    public class CategoryController : Controller
    {
        public readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult>Index()
        {
            List<Category> categories=await _context.Categories
                .Include(x=>x.Products)
                .ToListAsync();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Categories.Any(x => x.Name == category.Name);

            if (result) 
            {
                ModelState.AddModelError("Name", "Bu adda category artiq movcuddur");
                return View();
            }
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

            
        }
    }
}
