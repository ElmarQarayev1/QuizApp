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
 
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
		{
			
			_signInManager = signInManager;
			_userManager = userManager;
            _roleManager = roleManager;

		}
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

            var roleExists = await _roleManager.RoleExistsAsync("member");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("member"));
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(appUser);

                    foreach (var error in roleResult.Errors)
                    {
                        ModelState.AddModelError("", $"Role creation failed: {error.Description}");
                    }
                    return View(member);
                }
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
        public async Task<IActionResult> Login(MemberLoginViewModel member)
        {
            if (string.IsNullOrWhiteSpace(member.Password))
            {
                ModelState.AddModelError("Password", "Password must not be empty or whitespace");
                return View(member);
            }

            if (string.IsNullOrWhiteSpace(member.Email))
            {
                ModelState.AddModelError("Email", "Email must not be empty");
                return View(member);
            }

            AppUser appUser = await _userManager.FindByEmailAsync(member.Email);

            if (appUser == null)
            {
                ModelState.AddModelError("", "Email or Password is incorrect");
                return View(member);
            }

          
            //if (await _userManager.IsLockedOutAsync(appUser))
            //{
            //    ModelState.AddModelError("", "Your account has been temporarily locked due to multiple failed login attempts. Please try again later.");
            //    return View(member);
            //}

            
            var result = await _signInManager.PasswordSignInAsync(appUser, member.Password, isPersistent: false, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            //if (result.IsLockedOut)
            //{
            //    ModelState.AddModelError("", "Your account has been temporarily locked due to multiple failed login attempts. Please try again later.");
            //    return View(member);
            //}

            ModelState.AddModelError("", "Email or Password is incorrect");
            return View(member);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }


    }
}

