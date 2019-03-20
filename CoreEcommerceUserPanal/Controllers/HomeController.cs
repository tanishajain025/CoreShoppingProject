using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreEcommerceUserPanal.Models;

namespace CoreEcommerceUserPanal.Controllers
{
    public class HomeController : Controller
    {
        ShoppingprojectContext context = new ShoppingprojectContext();
        public IActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
        }
    }
}
