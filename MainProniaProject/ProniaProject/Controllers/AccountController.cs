using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProniaProject.Models;
using ProniaProject.Utilities.Enums;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM userVM)
        {
            if (!ModelState.IsValid) return View(userVM);
            if (RegisterValidator.IsEmailValid(userVM.Email)!=false)
            {
                ModelState.AddModelError("Email", "Email is not true");
                return View(userVM);
            }
            if (RegisterValidator.IsDigit(userVM.Name))
            {
                ModelState.AddModelError("Name", "You cannot include number in Name");
                return View(userVM);

            }
            if (RegisterValidator.IsDigit(userVM.Surname))
            {
                ModelState.AddModelError("Surname", "You cannot include number in Name");
                return View(userVM);
            }
            if (RegisterValidator.IsSymbol(userVM.Name))
            {
                ModelState.AddModelError("Name", "You cannot include symbol in Name");
                return View(userVM);
            }
            if (RegisterValidator.IsSymbol(userVM.Surname))
            {
                ModelState.AddModelError("Surname", "You cannot include number in Name");
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


        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM,string? returnUrl)
        {
            if(!ModelState.IsValid) return View();
            AppUser existedUser = await _userManager.FindByNameAsync(loginVM.UsernameOrEmail);
            if (existedUser is null)
            {
                existedUser=await _userManager.FindByEmailAsync(loginVM.UsernameOrEmail);
                if (existedUser is null)
                {
                    ModelState.AddModelError(String.Empty, "Username,Email or Account is Incorrect");
                    return View();
                }
            }
            var result=await _signInManager.PasswordSignInAsync(existedUser, loginVM.Password,loginVM.IsRemembered,true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Your Account Blocked because of Fail attempts Please try later");
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username,Email or Account is Incorrect");
                return View();
            }
            if (returnUrl is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }


        public async Task<IActionResult> CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(UserRoleHelper)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString(),
                    });
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}



