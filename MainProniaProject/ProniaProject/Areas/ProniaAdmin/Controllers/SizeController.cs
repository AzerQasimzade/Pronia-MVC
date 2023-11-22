using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SizeController : Controller
    {
        private readonly AppDbContext _context;
        public SizeController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Size> tags = await _context.Sizes
                .Include(x => x.ProductSizes)
                .ToListAsync();
            return View(tags);
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
                ModelState.AddModelError("Name", "Bu adda tag artiq movcuddur");
                return View();
            }
            await _context.Sizes.AddAsync(size);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
