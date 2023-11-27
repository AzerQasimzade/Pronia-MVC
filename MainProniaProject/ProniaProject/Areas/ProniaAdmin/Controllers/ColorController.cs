using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.Areas.ProniaAdmin.ViewModels;
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
        public async Task<IActionResult> Create(CreateColorVM colorVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Colors.Any(x => x.Name == colorVM.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda color artiq movcuddur");
                return View();
            }
            Color color=new Color
            {
               Name = colorVM.Name 
            };

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
            UpdateColorVM colorVM = new UpdateColorVM
            {
                Name= color.Name,
            };
            return View(colorVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateColorVM colorVM)
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
            bool result = await _context.Colors.AnyAsync(x => x.Name == colorVM.Name && x.Id != colorVM.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "We have Same Color Name.Please Try Another Name");
                return View();
            }
            existed.Name = colorVM.Name;
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
