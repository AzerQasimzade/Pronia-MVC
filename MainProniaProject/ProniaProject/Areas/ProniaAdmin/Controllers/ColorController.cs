using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]

    public class ColorController : Controller
    {
        public readonly AppDbContext _context;

        public ColorController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Color> colors = await _context.Colors
                .Include(x=>x.ProductColors)
                .ToListAsync();
            return View(colors);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Categories.Any(x => x.Name == color.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda category artiq movcuddur");
                return View();
            }
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}
