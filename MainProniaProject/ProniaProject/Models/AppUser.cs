using Microsoft.AspNetCore.Identity;
using ProniaProject.Utilities.Enums;

namespace ProniaProject.Models
{
    public class AppUser:IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public GenderHelper Gender {  get; set; }
    }
}
