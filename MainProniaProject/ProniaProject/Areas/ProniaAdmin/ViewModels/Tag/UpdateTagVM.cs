using ProniaProject.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaProject.Areas.ProniaAdmin.ViewModels
{
    public class UpdateTagVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public List<ProductTag>? ProductTags { get; set; }
    }
}
