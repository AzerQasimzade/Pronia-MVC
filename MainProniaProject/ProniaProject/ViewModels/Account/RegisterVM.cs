using ProniaProject.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProniaProject.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Name is required,Please Input Name")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        [MaxLength(25, ErrorMessage = "Name cannot be longer than 25 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Surname is required,Please Input Surname")]
        [MinLength(3, ErrorMessage = "Surname must be at least 3 characters long")]
        [MaxLength(25, ErrorMessage = "Surname cannot be longer than 25 characters")]
        public string Surname  { get; set; }
        [Required(ErrorMessage = "Username is required,Please Input Username")]
        [MinLength(4, ErrorMessage = "Username must be at least 4 characters long")]
        [MaxLength(50, ErrorMessage = "Username cannot be longer than 50 characters")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email Required,Please Input Email")]
        [MaxLength(256, ErrorMessage = "Username must be at least 4 characters long")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required,Please Input Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Password is required,Please Input Password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),ErrorMessage ="Confirm Pasword is not Matching with your Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public GenderHelper Gender { get; set; }
    }
}
