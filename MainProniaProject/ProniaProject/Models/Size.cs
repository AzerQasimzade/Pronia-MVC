using System.ComponentModel.DataAnnotations;

namespace ProniaProject.Models
{
    public class Size
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad mutleq daxil edilmelidir")]
        [MaxLength(25, ErrorMessage = "25 den uzun deyer gonderilmemelidir")]
        public string Name { get; set; }

        public List<ProductSize>? ProductSizes { get; set; }
    }
}
