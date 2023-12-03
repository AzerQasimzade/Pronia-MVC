using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.ViewModels;

namespace ProniaProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product= await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);
            if (product is null) return NotFound();
            BasketCookieItemVM item = new BasketCookieItemVM
            {
                Id = id,
                Count = 1
            };
            string json= JsonConvert.SerializeObject(item);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index),"Home");

        }

        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["Basket"]);
        }
    }
}
