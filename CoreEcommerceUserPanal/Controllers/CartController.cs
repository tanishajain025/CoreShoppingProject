using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEcommerceUserPanal.Helpers;
using CoreEcommerceUserPanal.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreEcommerceUserPanal.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        ShoppingprojectContext context = new ShoppingprojectContext();
        [Route("index")]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Products.ProductPrice * item.Quantity);
            return View();
        }
        [Route("buy /{id}")]
        public IActionResult Buy(int id)
        {
            if(SessionHelper.GetObjectFromJson<List<Item>>
                (HttpContext.Session,"cart") == null)
            {
                List<Item> cart = new List<Item>();
                cart.Add(new Item
                {
                    Products = context.Products.Find(id),
                    Quantity = 1
                });
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>
                    (HttpContext.Session, "cart");
                int index = isExist(id);
                if(index !=-1)
                {
                    cart[index].Quantity++;
;                }
                else
                {
                    cart.Add(new Item
                    {
                        Products = context.Products.Find(id),Quantity = 1 });
                    }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index","Home");

        }
        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>
                (HttpContext.Session, "cart");
            int index = isExist(id);
            if (index != -1)
            {
                cart[index].Quantity--;
;
            }
            else
            {
                cart.Remove(new Item
                {
                    Products = context.Products.Find(id),
                    Quantity = 1
                });
            }
            
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>
                (HttpContext.Session, "cart");
            for(int i=0;i<cart.Count;i++)
            {
                if(cart[i].Products.ProductId.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {
            var det=context.Products.Find(id);
            return View(det);
        }
        public IActionResult Checkout()
        {
            var checkout = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session,"cart");
            ViewBag.checkout = checkout;
            ViewBag.total = checkout.Sum(item => item.Products.ProductPrice * item.Quantity);
            ViewBag.totalitem = checkout.Count();
            TempData["total"] = ViewBag.total;
            return View();
        }
        [HttpPost]
        public IActionResult Checkout(Customers customer)
        {
            if (ModelState.IsValid)
            {
                context.Customers.Add(customer);
                context.SaveChanges();
                var amount = (TempData["total"]);
                Orders orders = new Orders()
                {
                    OrderPrice = Convert.ToSingle(amount),
                    OrderDate = DateTime.Now,
                    CustomerId = customer.CustomerId

                };
                context.Orders.Add(orders);
                context.SaveChanges();
                var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                List<OrderProducts> OrderProducts = new List<OrderProducts>();

                for (int i = 0; i < cart.Count; i++)
                {
                    OrderProducts orderproducts = new OrderProducts()
                    {
                        OrderId = orders.OrderId,
                        ProductId = cart[i].Products.ProductId,
                        Quantity = cart[i].Quantity
                    };
                    OrderProducts.Add(orderproducts);
                }
                OrderProducts.ForEach(n => context.OrderProducts.Add(n));
                context.SaveChanges();
                TempData["cust"] = customer.CustomerId;
                return RedirectToAction("Invoice");
            }
            return View();
        }
        [Route("Invoice")]
        public IActionResult Invoice()
        {
            int customerid = int.Parse(TempData["cust"].ToString());
            Customers customer = context.Customers.Where(x => x.CustomerId == customerid).SingleOrDefault();
            ViewBag.Customers = customer;

            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Products.ProductPrice * item.Quantity);
            
                return View();
        }
    }
}
