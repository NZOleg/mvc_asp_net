using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Store.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        public AccountController(UserManager<AppUser> userMgr,
        SignInManager<AppUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            //IdentitySeedData.EnsurePopulated(userMgr).Wait();
        }
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl; return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserModel details,
        string returnUrl)
        {
            Console.WriteLine(details.Email);
            Console.WriteLine(details.Password);

            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByEmailAsync(details.Email);
                if (user != null)
                {
                    
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result =
                    await signInManager.PasswordSignInAsync(
                    user, details.Password, false, false);
                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError(nameof(LoginUserModel.Email),
                "Invalid user or password");
            }
            return View(details);
        }
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Index() => View(GetData(nameof(Index)));
        [Authorize(Roles = "Users")]
        public IActionResult OtherAction() => View("Index",
        GetData(nameof(OtherAction)));
        private Dictionary<string, object> GetData(string actionName) =>
new Dictionary<string, object>
{
    ["Action"] = actionName,
    ["User"] = HttpContext.User.Identity.Name,
    ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated,
    ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
    ["In Users Role"] = HttpContext.User.IsInRole("Users")
};
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}
