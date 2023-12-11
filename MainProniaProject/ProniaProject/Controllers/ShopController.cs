using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.ViewModels;

namespace ProniaProject.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;

        public ShopController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string? search,int? order,int? categoryId)
        {
            IQueryable<Product> query= _context.Products.Include(pi=>pi.ProductImages).AsQueryable();

            switch (order)
            {
                case 1:
                    query = query.OrderBy(p => p.Name);
                    break;
                case 2:
                    query= query.OrderBy(p => p.Price);
                    break;
                case 3:
                    query=query.OrderBy(p => p.Id);
                    break;
            }
            if (!String.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }
            if (categoryId!=null)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }
         
            ShopVM shopVM = new ShopVM
            {
                Categories = await _context.Categories.Include(c=>c.Products).ToListAsync(),
                Products = await query.ToListAsync(),
                CategoryId=categoryId,
                Order=order,
                Search=search
            };
            return View(shopVM);
        }
    }
}
