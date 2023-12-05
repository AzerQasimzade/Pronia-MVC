using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaProject.Models;
using ProniaProject.Utilities.Extensions;
using ProniaProject.ViewModels;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ProniaProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {

            if (!ModelState.IsValid) return View(userVM);


            if (RegisterValidator.IsEmailValid(userVM.Email)==false)
            {
                ModelState.AddModelError("Email", "Email is not true");
                return View(userVM);
            }

            AppUser user = new AppUser
            {
                Name = RegisterValidator.Capitalize(userVM.Name),
                Email = userVM.Email,
                Surname = RegisterValidator.Capitalize(userVM.Surname),
                UserName = userVM.Username,
                Gender = userVM.Gender,
            };
            IdentityResult identityResult = await _userManager.CreateAsync(user, userVM.Password);
            if (!identityResult.Succeeded)
            {
                foreach (IdentityError error in identityResult.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}
