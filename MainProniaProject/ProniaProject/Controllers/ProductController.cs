﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DAL;
using ProniaProject.Models;
using ProniaProject.Utilities.Exceptions;
using ProniaProject.ViewModels;

namespace ProniaProject.Controllers
{

    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Detail(int id)
        {
            if (id<=0)
            {
                throw new WrongRequestException("Wrong Request!");
            }
            Product product=await _context.Products
                .Include(x=>x.ProductSizes)
                .ThenInclude(x=>x.Size)
                .Include(x=>x.Category)
                .Include(x=>x.ProductImages)
                .Include(x=>x.ProductColors)
                .ThenInclude(x=>x.Color)
                .Include(x=>x.ProductTags) 
                .ThenInclude(x=>x.Tag)              
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product is null)
            {
                throw new NotFoundException("Product cannot Found!");
            }
            List<Product> relatedproducts =await _context.Products   
                .Include(x=>x.Category)
                .Include(x => x.ProductImages)
                .Where(x=>x.Id!=id).Where(x => x.CategoryId==product.CategoryId).ToListAsync();
            ProductVM productVM = new ProductVM
            {

                Products = product,
                Relatedproducts = relatedproducts

            };

            return View(productVM);
        }
    }
}
