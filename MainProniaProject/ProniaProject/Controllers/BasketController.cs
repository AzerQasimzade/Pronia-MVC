using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
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
        public async Task<IActionResult> Index()
        {
            List<BasketİtemVM> items = new List<BasketİtemVM>();
            if (Request.Cookies["Basket"] is not null)
            {
                List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
                foreach (var cookie in cookies)
                {
                    Product product = await _context.Products
                        .Include(x=>x.ProductImages.Where(p=>p.IsPrimary==true))
                        .FirstOrDefaultAsync(p => p.Id == cookie.Id);
                    if(product is not null)
                    {
                        BasketİtemVM item = new BasketİtemVM
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Image=product.ProductImages.FirstOrDefault().Url,
                            Price = product.Price,
                            Count=cookie.Count,
                            Subtotal=product.Price*cookie.Count 
                        };
                        items.Add(item);
                    }
                }
            }
            return View(items);
        }
        public async Task<IActionResult> AddBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product= await _context.Products.FirstOrDefaultAsync(p=>p.Id==id);
            if (product is null) return NotFound();


            List<BasketCookieItemVM> basket;
            if (Request.Cookies["Basket"] is null)
            {
                basket = new List<BasketCookieItemVM>();
                BasketCookieItemVM item = new BasketCookieItemVM
                {
                    Id = id,
                    Count = 1
                };
                basket.Add(item);
            }
            else
            {
                basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);

                BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
                if (existed is null)
                {
                    BasketCookieItemVM item = new BasketCookieItemVM
                    {
                        Id = id,
                        Count = 1
                    };
                    basket.Add(item);
                }
                else
                {
                    existed.Count++;
                }
               
            }
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index),"Home");
        }

        public IActionResult GetBasket()
        {
            return Content(Request.Cookies["Basket"]);
        }
        public async Task<IActionResult> Remove(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            List<BasketCookieItemVM> basket;
            basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
            basket.Remove(existed);
            string json=JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket",json);
            return RedirectToAction(nameof(Index),"Basket");          
        }

        public async Task<IActionResult> PlusBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            List<BasketCookieItemVM> basket;
            basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
            if (existed is not null)
            {
                basket.FirstOrDefault(x => x.Id == id).Count++;
            }
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> MinusBasket(int id)
        {
            if (id <= 0) return BadRequest();
            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product is null) return NotFound();
            List<BasketCookieItemVM> basket;
            basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
            BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);     
            if (existed is not null)
            {
                basket.FirstOrDefault(x => x.Id == id).Count--;

                if (basket.FirstOrDefault(x => x.Id == id).Count == 0)
                {
                    basket.Remove(existed);
                }
            } 
            string json = JsonConvert.SerializeObject(basket);
            Response.Cookies.Append("Basket", json);
            return RedirectToAction(nameof(Index), "Basket");
        }
    }
}
