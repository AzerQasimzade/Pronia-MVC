using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.ViewModels;
using System.Security.Claims;

namespace ProniaProject.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly AppDbContext _context;
		private readonly UserManager<AppUser> _userManager;

		public HeaderViewComponent(AppDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
			_userManager = userManager;
		}
        public async Task<IViewComponentResult> InvokeAsync()
        {
			
			List<BasketİtemVM> items = new List<BasketİtemVM>();

			if (User.Identity.IsAuthenticated)
			{
				AppUser user = await _userManager.Users
				   .Include(u => u.BasketItems.Where(o => o.OrderId == null))
				   .ThenInclude(bi => bi.Product)
				   .ThenInclude(p => p.ProductImages.Where(i => i.IsPrimary == true))
				   .FirstOrDefaultAsync(us => us.Id == UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

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
			Dictionary<string, string> settings = await _context.Settings.ToDictionaryAsync(s => s.Key, s => s.Value);
			BasketHomeVM basketHomeVM = new BasketHomeVM
			{
				Settings= settings,
				BasketItems = items,
			};
			return View(basketHomeVM);

		}
	}
	
}
    

