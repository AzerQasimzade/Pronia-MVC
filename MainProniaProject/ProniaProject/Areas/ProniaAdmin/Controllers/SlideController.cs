using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.Areas.ProniaAdmin.ViewModels;
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
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {
           
            if (!ModelState.IsValid)
            {
                return View();
            }
            if (!slideVM.Photo.ValidateFileType(FileHelper.Image))
            {
                ModelState.AddModelError("Photo", "File tipi uygun deyil");
                return View();
            }
            if (!slideVM.Photo.ValidateSize(SizeHelper.mb))
            {
                ModelState.AddModelError("Photo", "File olcusu 1 mb den boyuk olmamalidir");
                return View();
            }
            string filename = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
            Slide slide = new Slide
            {
                Image = filename,
                Title = slideVM.Title,
                SubTitle = slideVM.SubTitle,
                Description = slideVM.Description,
                Order = slideVM.Order
            };
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
            if (id<=0)
            {
                return BadRequest();
            }
            Slide existed= await _context.Slides.FirstOrDefaultAsync(x=>x.Id == id);
            if(existed is null)
            {
                return NotFound();
            }
            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                Description = existed.Description,
                Order = existed.Order,
                SubTitle = existed.SubTitle,
                Title = existed.Title,
                Image = existed.Image
            };
            return View(slideVM);   
        }
        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateSlideVM slideVM)
        {
            Slide existed=await _context.Slides.FirstOrDefaultAsync(x=>x.Id==id);
            if(existed is null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            { 
                return View(slideVM);
            }
            if(slideVM.Photo is not null)
            {
                if (!slideVM.Photo.ValidateFileType(FileHelper.Image))
                {
                    ModelState.AddModelError("Photo", "File tipi uygun deyil");
                    return View(slideVM);
                }
                if (!slideVM.Photo.ValidateSize(SizeHelper.mb))
                {
                    ModelState.AddModelError("Photo", "File olcusu 1 mb den boyuk olmamalidir");
                    return View(slideVM);
                }
                string filename = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
                existed.Image.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
                existed.Image = filename;
            }
            existed.Title= slideVM.Title;
            existed.Description= slideVM.Description;
            existed.Order= slideVM.Order;
            existed.SubTitle= slideVM.SubTitle;
            await _context.SaveChangesAsync(); 
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            Slide slide = await _context.Slides
                .FirstOrDefaultAsync(x => x.Id == id);
            if (slide is null)
            {
                return NotFound();
            }
            return View(slide);
        }

    }
}
