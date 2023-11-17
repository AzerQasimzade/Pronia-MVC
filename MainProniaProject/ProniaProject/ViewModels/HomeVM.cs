using ProniaProject.Models;

namespace ProniaProject.ViewModels
{
    public class HomeVM
    {
        public List<Slide> Slides { get; set; }
        public List<Product> Products { get; set; }
        public List<Product> LatestProducts { get; set; }
    }
}
