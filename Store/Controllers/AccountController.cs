using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Models;
using Store.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            Console.WriteLine("------------------" + details.Email);
            Console.WriteLine("------------------" + details.Password);

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
                        if (user.ExprireDate != null) {
                             if (DateTime.Compare((DateTime)user.ExprireDate, DateTime.Now) <= 0)
                        {
                            user.ExprireDate = null;
                            user.MembershipType = null;
                            user.Message = "Your Membership has expired! Get a new one to get a discount!";
                        }
                    }
                        //return Redirect(returnUrl ?? "/");
                        return RedirectToAction("List", "Product");
                        
                       
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
            return RedirectToAction("List", "Product");
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
                    ["In Users Role"] = HttpContext.User.IsInRole("Users"),
                    ["MembershipType"] = CurrentUser().Result.MembershipType
                };

        [Authorize]
        public async Task<IActionResult> UserProps()
        {
            return View(await CurrentUser());
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserProps(
        [Required]MembershipTypes MembershipType)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await CurrentUser();
                user.MembershipType = MembershipType;
                await userManager.UpdateAsync(user);
                return RedirectToAction("Index");
            }
            return View(await CurrentUser());
        }
        private Task<AppUser> CurrentUser()
        {
            if (HttpContext.User.Identity.Name != null)
            {

                return userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }
            else
            {
                return null;
            }
        }

        [AllowAnonymous]
        public ViewResult Register() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            Console.WriteLine("------------------" + model.Email);
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email
                };
                IdentityResult result
                = await userManager.CreateAsync(user, model.Password);
                Console.WriteLine("------------------"+result);
                if (result.Succeeded)
                {
                    return RedirectToAction("List", "Product");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return RedirectToAction("List", "Product");
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

}
