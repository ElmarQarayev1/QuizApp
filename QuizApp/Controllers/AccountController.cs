using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Data;
using QuizApp.Models;
using QuizApp.Models.ViewModels;

namespace QuizApp.Controllers
{
	public class AccountController:Controller
	{
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_signInManager = signInManager;
			_userManager = userManager;
            _roleManager = roleManager;

		}

        //public async Task<IActionResult> CreateAdmin()
        //{
        //    AppUser appUser = new AppUser()
        //    {
        //        UserName = "admin",
        //        Email="admin@gmail.com"
        //    };
        //    var result = await _userManager.CreateAsync(appUser, "Admin123");
        //    await _userManager.AddToRoleAsync(appUser, "admin");
        //    return Json(result);
        //}

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(MemberRegisterViewModel member)
        {
            if (!ModelState.IsValid)
            {
                return View(member); 
            }

            var existingUserByEmail = await _userManager.FindByEmailAsync(member.Email);
            if (existingUserByEmail != null)
            {
                ModelState.AddModelError("Email", "Email is already taken");
                return View(member);
            }

            var existingUserByUsername = await _userManager.FindByNameAsync(member.UserName);
            if (existingUserByUsername != null)
            {
                ModelState.AddModelError("UserName", "UserName is already taken");
                return View(member);
            }

            AppUser appUser = new AppUser
            {
                UserName = member.UserName,
                Email = member.Email,
                FullName = member.FullName
            };

            var result = await _userManager.CreateAsync(appUser, member.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(member);
            }

            await _userManager.AddToRoleAsync(appUser, "member");
            return RedirectToAction("Login", "Account");
        }


        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }


        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(MemberLoginViewModel member, string returnUrl)
        {
            if (member.Password == null)
            {
                ModelState.AddModelError("Password", "Password mustn't be null");
                return View(member);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(member.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(member);
            }
     
            var result = await _signInManager.PasswordSignInAsync(appUser, member.Password, false, true);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(member);
            }

            return RedirectToAction("index", "home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


    }
}

