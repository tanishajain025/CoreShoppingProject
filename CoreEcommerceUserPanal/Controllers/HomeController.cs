using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreEcommerceUserPanal.Models;
using Microsoft.AspNetCore.Http;
using CoreEcommerceUserPanal.Helpers;

namespace CoreEcommerceUserPanal.Controllers
{
    
    public class HomeController : Controller
    {
        ShoppingProjectContext context = new ShoppingProjectContext();
        
         [HttpGet]
        public IActionResult Index()
        {
            var product = context.Products.ToList();
            int j = 0;
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int i = 0;
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    i++;
                }
                if (i != 0)
                {
                    foreach (var i1 in cart)
                    {
                        j++;
                    }
                    HttpContext.Session.SetString("cartitem", j.ToString());
                }
            }
            return View(product);
        }
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username != null && password != null && username.Equals("user") && password.Equals("123456"))
            {
                HttpContext.Session.SetString("uname", username);
                return View("Home");
            }
            else
            {
                ViewBag.Error = "Invalid Credentials";
                return View("Index");
            }
        }
        public IActionResult Display()
        {
            return View();
        }
        public IActionResult Front()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
    }
}
