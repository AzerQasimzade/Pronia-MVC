using ProniaProject.Models;

namespace ProniaProject.ViewModels
{
    public class ProductVM
    {
        public Product Products { get; set; }
        public List<Product> Relatedproducts { get; set; }
    }
}
