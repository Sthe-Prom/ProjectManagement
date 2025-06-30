using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Models;

namespace ProjectManagement.Controllers
{
    public class UserAccountController: Controller
    {
          //private properties
        private UserManager<User> UserManager;
        private SignInManager<User> signInManager;

        //Const
        public UserAccountController(UserManager<User> userManager_, SignInManager<User> signInManager_)
        {
            UserManager = userManager_;
            signInManager = signInManager_;
        }

        //Action Methods      
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [AllowAnonymous]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                User user = await UserManager.FindByEmailAsync(loginModel.Email);
                var userId = UserManager.GetUserId(User);
                User user_id = await UserManager.FindByIdAsync(userId);

                if (user != null)
                {
                    try
                    {
                        Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);

                        if (result.Succeeded)
                        {
                            return Redirect("/Home/Index");
                        }
                        else
                        {
                            return Redirect("/UserAccount/Login");
                            //return Content('could not login');
                        }
                    }
                    catch (Exception ex)
                    {
                        return Content(ex.Message);
                    }

                }
                // else
                // {

                    ModelState.AddModelError(nameof(LoginModel.Email), "Invalid username or password");
                    //return Content("Model is Valid.");
                   // return View(loginModel);
                //}
            }                        

            //ModelState.AddModelError(nameof(LoginModel.Email), "Invalid username or password");
            return View(loginModel);

        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login", "UserAccount");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}