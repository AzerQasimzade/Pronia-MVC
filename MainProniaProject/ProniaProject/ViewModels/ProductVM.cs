using ProniaProject.Models;

namespace ProniaProject.ViewModels
{
    public class ProductVM
    {
        public Product Products { get; set; }
        public List<Product> Relatedproducts { get; set; }

        public List<ProductTag> ProductTags { get; set; }

        public List<Tag> Tags { get; set; }

        public List<Color> Colors { get; set; }

        public List <ProductColor> ProductColors { get; set; }

        public List<ProductSize> ProductSizes { get; set; }

        public List<Size> Sizes { get; set; }   
    }
}
