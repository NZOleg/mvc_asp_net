using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Store.Infrastructure;
using Store.Models;
using Store.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.Controllers
{
    public class ProductController : Controller
    {
        private IProductRepository repository;
        public int PageSize = 6;
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;

        public ProductController(IProductRepository repo, UserManager<AppUser> userMgr,
        SignInManager<AppUser> signinMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            repository = repo;
        }

        public IEnumerable<Product> GetMembershipProducts()
        {
            return repository.Products
                    .Where(p => p.Category == "membership");
            
        }
        private Task<AppUser> CurrentUser() { 
            if(HttpContext.User.Identity.Name != null) {

                return userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            }
            else
            {
                return null;
            }
        }
        public ViewResult List(string category, int productPage = 1) {
            if(CurrentUser() != null) { 
            if (!String.IsNullOrEmpty(CurrentUser().Result.Message))
                {
                    ViewBag.Message = CurrentUser().Result.Message;
                    ViewBag.Message = "";
                }
            }

            return View(new ProductsListViewModel
            {
                Products = repository.Products
                    .Where(p => category == null || p.Category == category || p.Category == "membership")
                    .OrderBy(p => p.ProductID)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
repository.Products.Count() :
repository.Products.Where(e =>
e.Category == category).Count()
                },
                CurrentCategory = category
            });
        }

    }
}
