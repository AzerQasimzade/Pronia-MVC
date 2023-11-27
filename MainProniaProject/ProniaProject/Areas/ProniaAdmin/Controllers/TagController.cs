using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.Areas.ProniaAdmin.ViewModels;
using ProniaProject.DAL;
using ProniaProject.Models;
using System.Drawing;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]

    public class TagController : Controller
    {
        public readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Tag> tags = await _context.Tags
                .Include(x => x.ProductTags)
                .ToListAsync();
            return View(tags);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool result = _context.Tags.Any(x => x.Name == tagVM.Name);

            if (result)
            {
                ModelState.AddModelError("Name", "Bu adda tag artiq movcuddur");
                return View();
            }
            Tag tag = new Tag
            {
               Name = tagVM.Name

            };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag is null)
            {
                return NotFound();
            }
            UpdateTagVM tagVM = new UpdateTagVM
            {
                Name = tag.Name,
            };

            return View(tagVM);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateTagVM tagVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Tag existed = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);
            if (existed is null)
            {
                return NotFound(tagVM);
            }
            bool result = await _context.Tags.AnyAsync(x => x.Name == tagVM.Name && x.Id != tagVM.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "We have Same Tag Name.Please Try Another Name");
                return View();
            }
            existed.Name = tagVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Tag tag = await _context.Tags.FirstOrDefaultAsync(x => x.Id == id);

            if (tag is null)
            {
                return NotFound();
            }
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            Tag tag = await _context.Tags
                .Include(x => x.ProductTags)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (tag is null)
            {
                return NotFound();
            }
            return View(tag);
        }

    }
}


