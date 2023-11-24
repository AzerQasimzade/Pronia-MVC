using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SizeController : Controller
    {
        public readonly AppDbContext _context;

        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> sizes = await _context.Sizes
                .Include(x => x.ProductSizes)
                .ToListAsync();
            return View(sizes);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Sizes.Any(x => x.Name == size.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda size artiq movcuddur");
                return View();
            }
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Size size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (size is null)
            {
                return NotFound();
            }
            return View(size);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, Size size)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Size existed = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            bool result = await _context.Sizes.AnyAsync(x => x.Name == size.Name && x.Id != size.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "We have Same Size Name.Please Try Another Name");
                return View();
            }
            existed.Name = size.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Size size = await _context.Sizes.FirstOrDefaultAsync(x => x.Id == id);

            if (size is null)
            {
                return NotFound();
            }
            _context.Sizes.Remove(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
