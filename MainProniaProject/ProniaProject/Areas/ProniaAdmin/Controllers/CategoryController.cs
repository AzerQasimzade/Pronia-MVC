using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.Areas.ProniaAdmin.ViewModels;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.ViewModels;

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
        public async Task<IActionResult>Index(int page=1)
        {
            if (page<1)
            {
                return BadRequest();
            }
            int count = await _context.Categories.CountAsync();
            if (page > Math.Ceiling((double)count / 2))
            {
                return BadRequest();
            }
            List<Category> categories=await _context.Categories.Skip((page - 1) * 2).Take(2)
                .Include(x=>x.Products)
                .ToListAsync();
            PaginationVM<Category> paginationVM = new PaginationVM<Category>()
            {
                TotalPage = Math.Ceiling((double)count / 2),
                CurrentPage = page,
                Items = categories
            };
            return View(paginationVM);
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

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Category category=await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if(category is null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,Category category)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Category existed = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            bool result=await _context.Categories.AnyAsync(x => x.Name == category.Name && x.Id!=category.Id);
            if (result)
            {
                ModelState.AddModelError("Name", "We have Same Category Name.Please Try Another Name");
                return View();
            }
            existed.Name= category.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }   
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Category category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
            {
                return NotFound();
            }
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();  
            return RedirectToAction(nameof(Index));         
        }
        public async Task<IActionResult> Details(int id)
        {
            Category category = await _context.Categories
                .Include(x=>x.Products)
                .ThenInclude(x=>x.ProductTags)
                .ThenInclude(x=>x.Tag)
                .Include(x => x.Products)
                .ThenInclude(x=>x.ProductImages) 
                .FirstOrDefaultAsync(x => x.Id == id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }
    }
}
