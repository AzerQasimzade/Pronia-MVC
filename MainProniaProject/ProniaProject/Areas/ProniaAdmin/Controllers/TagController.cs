using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class TagController : Controller
    {

        private readonly AppDbContext _context;
        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags
                .Include(x =>x.ProductTags)
                .ToListAsync();
            return View(tags);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Tags.Any(x => x.Name == tag.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda tag artiq movcuddur");
                return View();
            }
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }
    }
}

