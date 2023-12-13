using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.Areas.ProniaAdmin.ViewModels;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.Utilities.Enums;
using ProniaProject.Utilities.Extensions;
using System;
using System.Drawing;

namespace ProniaProject.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    [Authorize(Roles = "Admin,Moderator")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            if (page<1)
            {
                return BadRequest();
            }       
            int count = await _context.Products.CountAsync();
            if (page > Math.Ceiling((double)count / 6))
            {
                return BadRequest();
            }
            List<Product> products = await _context.Products.Skip((page-1)*6).Take(6)
                .Include(x => x.Category)
                .Include(x => x.ProductImages
                .Where(pi => pi.IsPrimary == true))
                .ToListAsync();

            PaginationVM<Product> paginationVM = new PaginationVM<Product>()
            {
                TotalPage = Math.Ceiling((double)count / 6),
                CurrentPage = page,
                Items = products
            };
            
            return View(paginationVM);
        }
        [Authorize(Roles = "Admin,Moderator")]

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

            //---------------MAIN PHOTO CHECKING------------------
            if (!productVM.MainPhoto.ValidateFileType(FileHelper.Image))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("MainPhoto", "File Type is not Matching");
                return View(productVM);
            }
            if (!productVM.MainPhoto.ValidateSize(SizeHelper.gb))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("MainPhoto", "File Size is not Suitable");
                return View(productVM);
            }
            //---------------HOVER PHOTO CHECKING------------------
            if (!productVM.HoverPhoto.ValidateFileType(FileHelper.Image))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "File Type is not Matching");
                return View(productVM);
            }
            if (!productVM.HoverPhoto.ValidateSize(SizeHelper.gb))
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                ModelState.AddModelError("HoverPhoto", "File Size is not Suitable");
                return View(productVM);
            }

            ProductImage main = new ProductImage
            {
                IsPrimary = true,
                Url = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name

            };

            ProductImage hover = new ProductImage
            {
                IsPrimary = false,
                Url = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images"),
                Alternative = productVM.Name
            };


            Product product = new Product
            {
                Name = productVM.Name,
                SKU = productVM.SKU,
                Description = productVM.Description,
                Price = productVM.Price,
                CategoryId = (int)productVM.CategoryId,
                ProductTags = new List<ProductTag>(),
                ProductColors = new List<ProductColor>(),
                ProductSizes = new List<ProductSize>(),
                ProductImages = new List<ProductImage> { main, hover }
            };
            TempData["Message"] = "";
            foreach (IFormFile photo in productVM.Photos ?? new List<IFormFile>())
            {
                if (!photo.ValidateFileType(FileHelper.Image))
                {
                    TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Type is not suitable,That's why creating file's Mission Failed </div>";
                    continue;
                }

                if (!photo.ValidateSize(SizeHelper.gb))
                {
                    TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Size is not suitable,That's why creating file's Mission Failed </div>";
                    continue;
                }
                product.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Alternative = productVM.Name,
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
                });
            }






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
                .Include(x => x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductTags)
                .ThenInclude(pt => pt.Tag)
                .Include(x => x.ProductColors)
                .ThenInclude(x => x.Color)
                .Include(x => x.ProductSizes)
                .ThenInclude(x => x.Size)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize(Roles = "Admin,Moderator")]

        public async Task<IActionResult> Update(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            Product product = await _context.Products
                .Include(x => x.ProductTags)
                .Include(x => x.ProductColors)
                .Include(x => x.ProductSizes)
                .Include(x => x.ProductImages)
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
                TagIds = product.ProductTags.Select(x => x.TagId).ToList(),
                ColorIds = product.ProductColors.Select(x => x.ColorId).ToList(),
                SizeIds = product.ProductSizes.Select(x => x.SizeId).ToList(),
                Categories = await _context.Categories.ToListAsync(),
                Tags = await _context.Tags.ToListAsync(),
                Colors = await _context.Colors.ToListAsync(),
                Sizes = await _context.Sizes.ToListAsync(),
                ProductImages = product.ProductImages,
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, UpdateProductVM productVM)
        {
            Product existed = await _context.Products
                .Include(x => x.ProductTags)
                .Include(x => x.ProductColors)
                .Include(x => x.ProductSizes)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(y => y.Id == id);
            if (!ModelState.IsValid)
            {
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                return View(productVM);
            }

            if (existed is null)
            {
                return NotFound();
            }
            if (productVM.MainPhoto is not null)
            {
                if (!productVM.MainPhoto.ValidateFileType(FileHelper.Image))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;
                    ModelState.AddModelError("MainPhoto", "File Type is not Matching");
                    return View(productVM);
                }
                if (!productVM.MainPhoto.ValidateSize(SizeHelper.gb))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;
                    ModelState.AddModelError("MainPhoto", "File Size is not Suitable");
                    return View(productVM);
                }
            }
            if (productVM.HoverPhoto is not null)
            {
                if (!productVM.HoverPhoto.ValidateFileType(FileHelper.Image))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;
                    ModelState.AddModelError("HoverPhoto", "File Type is not Matching");
                    return View(productVM);
                }
                if (!productVM.HoverPhoto.ValidateSize(SizeHelper.gb))
                {
                    productVM.Categories = await _context.Categories.ToListAsync();
                    productVM.Tags = await _context.Tags.ToListAsync();
                    productVM.Colors = await _context.Colors.ToListAsync();
                    productVM.Sizes = await _context.Sizes.ToListAsync();
                    productVM.ProductImages = existed.ProductImages;
                    ModelState.AddModelError("HoverPhoto", "File Size is not Suitable");
                    return View(productVM);
                }
            }
            bool result = await _context.Categories.AnyAsync(x => x.Id == productVM.CategoryId);
            if (!result)
            {
                productVM.Tags = await _context.Tags.ToListAsync();
                productVM.Categories = await _context.Categories.ToListAsync();
                productVM.Colors = await _context.Colors.ToListAsync();
                productVM.Sizes = await _context.Sizes.ToListAsync();
                productVM.ProductImages = existed.ProductImages;
                ModelState.AddModelError("CategoryId", "We have not so Category");
                return View();
            }
            foreach (var pTag in existed.ProductTags)
            {
                if (!productVM.TagIds.Exists(tId => tId == pTag.TagId))
                {
                    _context.ProductTags.Remove(pTag);
                }
            }
            foreach (int tId in productVM.TagIds)
            {
                if (!existed.ProductTags.Any(pt => pt.TagId == tId))
                {
                    existed.ProductTags.Add(new ProductTag
                    {
                        TagId = tId
                    });
                }
            }

            if (productVM.MainPhoto is not null)
            {
                string filename = await productVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existedImg = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == true);
                existedImg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(existedImg);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = true,
                    Alternative=productVM.Name,
                    Url= filename
                });
            }
            if (productVM.HoverPhoto is not null)
            {
                string filename = await productVM.HoverPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images");
                ProductImage existedImg = existed.ProductImages.FirstOrDefault(pi => pi.IsPrimary == false);
                existedImg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(existedImg);
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = false,
                    Alternative = productVM.Name,
                    Url = filename
                });
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
            if (productVM.ImageIds is  null)
            {
                productVM.ImageIds = new List<int>();
            }
            List<ProductImage> removeable = existed.ProductImages.Where(pi =>!productVM.ImageIds.Exists(imgId=>imgId==pi.Id)&&pi.IsPrimary==null).ToList();
            foreach (ProductImage reimg in removeable)
            {
                reimg.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
                existed.ProductImages.Remove(reimg);

            }
            TempData["Message"] = "";
            foreach (IFormFile photo in productVM.Photos ?? new List<IFormFile>())
            {
                if (!photo.ValidateFileType(FileHelper.Image))
                {
                    TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Type is not suitable,That's why creating file's Mission Failed </div>";
                    continue;
                }

                if (!photo.ValidateSize(SizeHelper.gb))
                {
                    TempData["Message"] += $"<div class=\"alert alert-danger\" role=\"alert\"> {photo.FileName} file's Size is not suitable,That's why creating file's Mission Failed </div>";
                    continue;
                }
                existed.ProductImages.Add(new ProductImage
                {
                    IsPrimary = null,
                    Alternative = productVM.Name,
                    Url = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "website-images")
                });
            }


            existed.Name = productVM.Name;
            existed.Price = productVM.Price;
            existed.SKU = productVM.SKU;
            existed.Description = productVM.Description;
            existed.CategoryId = productVM.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int id) 
        {
            if (id <= 0) return BadRequest();
            Product product=await _context.Products.Include(x=>x.ProductImages).FirstOrDefaultAsync(x=>x.Id==id);
            if (product==null) return NotFound();
            foreach (var image in product.ProductImages ?? new List<ProductImage>())
            {
                image.Url.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}