using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AuthController : Controller
    {
        private SignInManager<WorldUser> _signInManager;

        public AuthController(SignInManager<WorldUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Trips", "App");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel viewModel, string returnUrl)
        {
            // 1. Attempt to use _signInManager with credentials provided on viewModel
            // 2. If successful, check if there was a return URL provided from the attempted access
            //    of a Action/View with an [Authorized] metadata. Redirect there if so, otherwise
            //    redirect to AppController / Trips
            // 3. If unsuccessful, add model error and return view.
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(viewModel.Username, 
                                                                      viewModel.Password, 
                                                                      true, false);
                if (signInResult.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(returnUrl))
                    {
                        RedirectToAction("Trips", "App");
                    }else
                    {
                        Redirect(returnUrl);
                    }                   
                }else
                {
                    ModelState.AddModelError("", "Username or Password Incorrect");
                }
            }
            return View();
        }
        public async Task<ActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "App");
        }
    }
}
