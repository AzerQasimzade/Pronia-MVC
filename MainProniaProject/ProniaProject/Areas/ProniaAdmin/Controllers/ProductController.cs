using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.Areas.ProniaAdmin.ViewModels;
using ProniaProject.DAL;
using ProniaProject.Models;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products=await _context.Products
                .Include(x=>x.Category)

                .Include(x=>x.ProductImages
                .Where(pi=>pi.IsPrimary==true))

                .ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {

            CreateProductVM productVM = new CreateProductVM
            {
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
            };
            return View(productVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productVM)
        {
         

            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                return View(productVM);
            }
            bool result2 = await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId);
            if (!result2)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId", "We have not so Category with this Id");
                return View(productVM);
            }
            foreach (var tagId in productVM.TagIds)
            {
                bool TagResult = await _context.Tags.AnyAsync(x => x.Id == tagId);
                if (!TagResult)
                {
                    ModelState.AddModelError("TagIds", "We have not so Tag with this Id");
                    return View();
                }
            }
            foreach (var colorId in productVM.ColorIds)
            {
                bool ColorResult = await _context.Colors.AnyAsync(x => x.Id == colorId);
                if (!ColorResult)
                {
                    ModelState.AddModelError("ColorIds", "We have not so Color with this Id");
                    return View();
                }
            }
            foreach (var sizeId in productVM.SizeIds)
            {
                bool SizeResult = await _context.Sizes.AnyAsync(x => x.Id == sizeId);
                if (!SizeResult)
                {
                    ModelState.AddModelError("SizeIds", "We have not so Size with this Id");
                    return View();
                }
            }
            Product product = new Product
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                Description = productVM.Description,
                Price = productVM.Price,
                CategoryId = (int)productVM.CategoryId,
                ProductTags = new List<ProductTag>(),
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>()
            };
            foreach (int tagId in productVM.TagIds)
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagId,
                };
                product.ProductTags.Add(productTag);
            }
            foreach (int colorId in productVM.ColorIds)
            {
                ProductColor productColor = new ProductColor
                {
                    ColorId = colorId,
                };
                product.ProductColors.Add(productColor);
            }
            foreach (int sizeId in productVM.SizeIds)
            {
                ProductSize productSize = new ProductSize
                {
                    SizeId = sizeId,
                };
                product.ProductSizes.Add(productSize);
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        public async Task<IActionResult> Details(int id)
        {
            Product product = await _context.Products
                .Include(x=>x.Category)
                .Include(x=>x.ProductImages)
                .Include(x=>x.ProductTags)
                .ThenInclude(pt=>pt.Tag)
                .Include(x=>x.ProductColors)
                .ThenInclude(x=>x.Color)
                .Include(x => x.ProductSizes)
                .ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            return View(product);
        }
        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            { 
                return BadRequest();
            }
            Product product = await _context.Products
                .Include(x=>x.ProductTags)
                .Include(x=>x.ProductColors)
                .Include(x=>x.ProductSizes)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            UpdateProductVM productVM = new UpdateProductVM
            {
                Name = product.Name,
                Price = product.Price,
                SKU = product.SKU,
                Description = product.Description,
                CategoryId = product.CategoryId,
                TagIds= product.ProductTags.Select(x => x.TagId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(), 
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id,UpdateProductVM productVM)
        {
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync(); 
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes= await _context.Sizes.ToListAsync();
                return View(productVM);
            }
            Product existed=await _context.Products
                .Include(x=>x.ProductTags)
                .Include(x=>x.ProductColors)
                .Include(x=>x.ProductSizes)
                .FirstOrDefaultAsync(y => y.Id == id);     
            if (existed is null)
            {
                return NotFound();
            }
            bool result =await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId);
            if (!result)
            {
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("CategoryId", "We have not so Category");
                return View();
            }
            foreach (var pTag in existed.ProductTags)
            {
                if (!productVM.TagIds.Exists(tId=>tId==pTag.TagId))
                {
                    _context.ProductTags.Remove(pTag);
                }
            }
            foreach(int tId in productVM.TagIds)
            {
                if (!existed.ProductTags.Any(pt => pt.TagId == tId))
                {
                    existed.ProductTags.Add(new ProductTag
                    {
                        TagId=tId 
                    });
                }
            }
            //--------------------Color
            foreach (var pColor in existed.ProductColors)
            {
                if (!productVM.ColorIds.Exists(cId => cId == pColor.ColorId))
                {
                    _context.ProductColors.Remove(pColor);
                }
            }
            foreach (int cId in productVM.ColorIds)
            {
                if (!existed.ProductColors.Any(pc => pc.ColorId == cId))
                {
                    existed.ProductColors.Add(new ProductColor
                    {
                        ColorId = cId
                    });
                }
            }
            //-------------------Size
            foreach (var pSize in existed.ProductSizes)
            {
                if (!productVM.SizeIds.Exists(sId => sId == pSize.SizeId))
                {
                    _context.ProductSizes.Remove(pSize);
                }
            }
            foreach (int sId in productVM.SizeIds)
            {
                if (!existed.ProductSizes.Any(ps => ps.SizeId == sId))
                {
                    existed.ProductSizes.Add(new ProductSize
                    {
                        SizeId = sId
                    });
                }
            }
            existed.Name = productVM.Name;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.Description = productVM.Description;
            existed.CategoryId = productVM.CategoryId;
           
            await _context. SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}