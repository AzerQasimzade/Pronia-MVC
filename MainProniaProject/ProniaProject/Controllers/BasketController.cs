using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.ContentModel;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.ViewModels;
using System.Security.Claims;

namespace ProniaProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public BasketController(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
		}
        public async Task<IActionResult> Index()
        {
            List<BasketİtemVM> items = new List<BasketİtemVM>();
            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(au => au.BasketItems)
                    .ThenInclude(bi => bi.Product)
                    .ThenInclude(pt => pt.ProductImages.Where(pi => pi.IsPrimary == true))
                    .FirstOrDefaultAsync(u=>u.Id==User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (var item in user.BasketItems)
                {
                    items.Add(new BasketİtemVM
                    {
                        Id = item.ProductId,
                        Price = item.Product.Price,
                        Name = item.Product.Name,
                        Count = item.Count,
                        Subtotal = item.Count * item.Product.Price,
                        Image = item.Product.ProductImages.FirstOrDefault()?.Url

                    });
                }
            }
            else
            {
				if (Request.Cookies["Basket"] is not null)
				{
					List<BasketCookieItemVM> cookies = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
					foreach (var cookie in cookies)
					{
						Product product = await _context.Products
							.Include(x => x.ProductImages.Where(p => p.IsPrimary == true))
							.FirstOrDefaultAsync(p => p.Id == cookie.Id);
						if (product is not null)
						{
							BasketİtemVM item = new BasketİtemVM
							{
								Id = product.Id,
								Name = product.Name,
								Image = product.ProductImages.FirstOrDefault().Url,
								Price = product.Price,
								Count = cookie.Count,
								Subtotal = product.Price * cookie.Count
							};
							items.Add(item);
						}
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


            if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(x => x.BasketItems)
                    .FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                    
                if(user is null) return NotFound();
                var item= user.BasketItems.FirstOrDefault(x=>x.ProductId==product.Id);
                if (item is null)
                {
                    item = new BasketItem
                    {
                        AppUserId = user.Id,
                        Price = product.Price,
                        Count = 1,
                        ProductId=product.Id,

                    };
                    user.BasketItems.Add(item);
                }
                else
                {
                    item.Count++;
                }             
                await _context.SaveChangesAsync();  
            }
            else
            {
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
			}

           
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
            if (User.Identity.IsAuthenticated)
            {

				AppUser user = await _userManager.Users
                    .Include(u => u.BasketItems)
                    .FirstOrDefaultAsync(bi => bi.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                
				var excisted = user.BasketItems                  
                    .FirstOrDefault(x => x.ProductId == product.Id);
                if (excisted is null) return NotFound();
                user.BasketItems.Remove(excisted);
                 _context.SaveChanges();		
            }
            else
            {
				basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
				BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
				basket.Remove(existed);
				string json = JsonConvert.SerializeObject(basket);
				Response.Cookies.Append("Basket", json);
			}
            return RedirectToAction(nameof(Index),"Basket");          
        }
        public async Task<IActionResult> PlusBasket(int id)
        {
			if (id <= 0) return BadRequest();
			Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
			if (product is null) return NotFound();
			List<BasketCookieItemVM> basket;
			if (User.Identity.IsAuthenticated)
            {
                AppUser user = await _userManager.Users
                    .Include(u => u.BasketItems).FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (user == null) return NotFound();
                user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id).Count++;
                _context.SaveChanges();           
            }
            else
            {
				basket = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(Request.Cookies["Basket"]);
				BasketCookieItemVM existed = basket.FirstOrDefault(x => x.Id == id);
				if (existed is not null)
				{
					basket.FirstOrDefault(x => x.Id == id).Count++;
				}
				string json = JsonConvert.SerializeObject(basket);
				Response.Cookies.Append("Basket", json);
			}                      
            return RedirectToAction(nameof(Index), "Basket");
        }
        public async Task<IActionResult> MinusBasket(int id)
        {
			if (id <= 0) return BadRequest();
			Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
			if (product is null) return NotFound();
			List<BasketCookieItemVM> basket;
			if (User.Identity.IsAuthenticated)
            {
				AppUser user = await _userManager.Users
					.Include(u => u.BasketItems).FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
				if (user == null) return NotFound();
				var excisted = user.BasketItems
					.FirstOrDefault(x => x.ProductId == product.Id);
				user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id).Count--;
                if(user.BasketItems.FirstOrDefault(x => x.ProductId == product.Id).Count == 0)
                {
					user.BasketItems.Remove(excisted);
				}
				_context.SaveChanges();

			}
            else
            {
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
			}
            
           
            return RedirectToAction(nameof(Index), "Basket");
        }
    }
}
