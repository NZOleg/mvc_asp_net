using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Controllers
{
    public class OrderController : Controller
    {
        private IOrderRepository repository;
        private Cart cart;
        private UserManager<AppUser> userManager;
        public OrderController(IOrderRepository repoService, Cart cartService, UserManager<AppUser> userMgr)
        {
            repository = repoService;
            cart = cartService;
            userManager = userMgr;
        }
        [Authorize]
        public ViewResult List() =>
View(repository.Orders.Where(o => !o.Shipped));
        [HttpPost]
        [Authorize]
        public IActionResult MarkShipped(int orderID)
        {
            Models.Order order = repository.Orders
            .FirstOrDefault(o => o.OrderID == orderID);
            if (order != null)
            {
                order.Shipped = true;
                repository.SaveOrder(order);
            }
            return RedirectToAction(nameof(List));
        }

        public ViewResult Checkout() => View(new Models.Order());
        [HttpPost]
        public IActionResult Checkout(Models.Order order)
        {
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
            }
            if (ModelState.IsValid)
            {
                order.Lines = cart.Lines.ToArray();
                repository.SaveOrder(order);
                return RedirectToAction(nameof(Completed));
            }
            else
            {
                return View(order);
            }
        }
        private Task<AppUser> CurrentUser =>
        userManager.FindByNameAsync(HttpContext.User.Identity.Name);
        [HttpPost]
        public IActionResult MembershipCheckout(string membership)
        {
            Console.WriteLine("----------" + membership);
            MembershipTypes MembershipType;
            switch (membership)
            {

                case "Gold":
                    MembershipType = MembershipTypes.Gold;
                    break;
                case "Silver":
                    MembershipType = MembershipTypes.Silver;
                    break;
                case "Bronze":
                    MembershipType = MembershipTypes.Bronze;
                    break;
                default:
                    MembershipType = MembershipTypes.Gold;
                    break;
            }
            AppUser user = CurrentUser.Result;
            user.MembershipType = MembershipType;
            user.ExprireDate = DateTime.Now.AddMonths(12);
            userManager.UpdateAsync(user);
            Console.WriteLine("----------" + user.MembershipType);
            return RedirectToAction("List", "Product");

        }

        public ViewResult Completed()
        {
            cart.Clear();
            return View();
        }
    }
}
