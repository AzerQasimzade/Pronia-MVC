using ProniaProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaProject.Areas.ProniaAdmin.ViewModels
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        [Required]
        public int? CategoryId { get; set; }
    }
}
