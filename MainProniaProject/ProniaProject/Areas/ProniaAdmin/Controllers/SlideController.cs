using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.Utilities.Enums;
using ProniaProject.Utilities.Extensions;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class SlideController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SlideController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides = await _context.Slides.ToListAsync();
            return View(slides);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (slide.Photo is null)
            {
                ModelState.AddModelError("Photo", "Shekil mutlew secilmelidir");
                return View();
            }
            if (!slide.Photo.ValidateFileType(FileHelper.Image))
            {
                ModelState.AddModelError("Photo", "File tipi uygun deyil");
                return View();
            }
            if (!slide.Photo.ValidateSize(SizeHelper.mb))
            {
                ModelState.AddModelError("Photo", "File olcusu 1 mb den boyuk olmamalidir");
                return View();
            }
            slide.Image = await slide.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Slide existed = await _context.Slides.FirstOrDefaultAsync(x => x.Id == id);
            if (existed is null)
            {
                return NotFound();
            }
            existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
            _context.Slides.Remove(existed);    
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int id)
        {

        }














        //public async Task<IActionResult> Details(int id)
        //{
        //    Slide slider = await _context.Slides
        //        .Include(x => x.)
        //        .ThenInclude(x => x.Product)
        //        .ThenInclude(x => x.ProductImages)
        //        .FirstOrDefaultAsync(x => x.Id == id);

        //    if (slider is null)
        //    {
        //        return NotFound();
        //    }
        //    return View(tag);
        //}
    }
}
