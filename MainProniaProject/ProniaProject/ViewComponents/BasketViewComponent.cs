using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.ViewModels;

namespace ProniaProject.ViewComponents
{
    public class BasketViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;

        public BasketViewComponent(AppDbContext context)
        {
            _context = context;
        }   
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<BasketİtemVM> items = new List<BasketİtemVM>();
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
            return View(items);

        }
    }
}
