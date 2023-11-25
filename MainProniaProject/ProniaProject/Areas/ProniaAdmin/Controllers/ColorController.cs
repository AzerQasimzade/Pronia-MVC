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

            bool result = _context.Colors.Any(x => x.Name == color.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color artiq movcuddur");
                return View();
            }
            await _context.Colors.AddAsync(color);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Color color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);

            if (color is null)
            {
                return NotFound();
            }
            return View(color);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Color color)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Color existed = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            bool result = await _context.Colors.AnyAsync(x => x.Name == color.Name && x.Id != color.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "We have Same Color Name.Please Try Another Name");
                return View();
            }
            existed.Name = color.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Color color = await _context.Colors.FirstOrDefaultAsync(x => x.Id == id);

            if (color is null)
            {
                return NotFound();
            }
            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            Color color = await _context.Colors
                .Include(x => x.ProductColors)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (color is null)
            {
                return NotFound();
            }
            return View(color);
        }
    }
}
