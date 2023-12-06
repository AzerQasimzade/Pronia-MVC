using System.ComponentModel.DataAnnotations;

namespace ProniaProject.ViewModels
{
    public class LoginVM
    {
        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string UsernameOrEmail { get; set; }
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemembered { get; set; }
    }
}
