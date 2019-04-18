using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEcommerceUserPanal.Helpers;
using CoreEcommerceUserPanal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe;

namespace CoreEcommerceUserPanal.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        ShoppingProjectContext context = new ShoppingProjectContext();
        [Route("index")]

        public IActionResult Index()
        {

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
                    ViewBag.cart = cart;
                    ViewBag.total = cart.Sum(item => item.Products.ProductPrice * item.Quantity);
                    if (SessionHelper.GetObjectFromJson<Customers>(HttpContext.Session, "cust") == null)
                    {
                        ViewBag.i = 0;
                    }
                    else
                    {
                        ViewBag.i = 1;
                    }
                    return View();
                }

            }
            return View("GoBack");
        }
        [Route("buy /{id}")]
        public IActionResult Buy(int id)
        {
            Products p = context.Products.Find(id);
            if (p.ProductQty < 1)
            {
                return RedirectToAction("OutofStock", "cart", new { @id = id });
            }
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
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
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                    ;
                }
                else
                {
                    cart.Add(new Item
                    {
                        Products = context.Products.Find(id),
                        Quantity = 1
                    });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index", "Home");

        }
        [Route("OutofStock/{id}")]
        public IActionResult OutofStock(int id)
        {
            var detail = context.Products.Find(id);
            var cid = context.Products.Find(id);
            ViewBag.cname = context.Categories.Find(cid.ProductCategoryId);
            return View(detail);

        }
        [Route("Remove/{id}")]

        public IActionResult Remove(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            int j = int.Parse(HttpContext.Session.GetString("cartitem"));
            int i = 0;
            foreach (var item in cart)
            {
                i++;
            }
            if (i != 0)
            {
                j--;
                HttpContext.Session.SetString("cartitem", j.ToString());
            }
            else
            {
                HttpContext.Session.Remove("cartitem");
                return View("GoBack");
            }
            return RedirectToAction("Index");

        }
        [Route("GoBack")]
        public IActionResult GoBack()
        {
            return View();
        }

        private int isExist(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>
                (HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Products.ProductId.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }
        [Route("Details/{id}")]
        public IActionResult Details(int id)
        {

            var detail = context.Products.Find(id);
            var cid = context.Products.Find(id);
            ViewBag.cname = context.Categories.Find(cid.ProductCategoryId);
            return View(detail);
        }
        [Route("checkout")]
        [HttpGet]
        public IActionResult Checkout()
        {
            int i = 0;
            ViewBag.i = i;
            var checkout = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.checkout = checkout;
            ViewBag.total = checkout.Sum(item => item.Products.ProductPrice * item.Quantity);
            string cust = HttpContext.Session.GetString("uname");
            Customers cus = context.Customers.Where(x => x.UserName == cust).SingleOrDefault();
            ViewBag.cus = cus;
            ViewBag.totalitem = checkout.Count();
            TempData["total"] = ViewBag.total;
            return View();
        }
        [Route("checkout")]
        [HttpPost]
        public IActionResult Checkout(Customers customer1, string stripeEmail, string stripeToken)
        {

            var c = context.Customers.Where(x => x.UserName == customer1.UserName).SingleOrDefault();
            c.FirstName = customer1.FirstName;
            c.LastName = customer1.LastName;
            c.UserName = customer1.UserName;
            c.EmailId = customer1.EmailId;
            c.Address = customer1.Address;
            c.PhoneNo = customer1.PhoneNo;
            c.Country = customer1.Country;
            c.State = customer1.State;
            c.Zip = customer1.Zip;
            context.SaveChanges();
            var amount1 = (TempData["total"]);
            Orders orders = new Orders()
            {
                OrderPrice = Convert.ToSingle(amount1),
                OrderDate = DateTime.Now,
                CustomerId = c.CustomerId

            };
            context.Orders.Add(orders);

            context.SaveChanges();
            TempData["orderId"] = orders.OrderId;
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
            TempData["cust"] = c.CustomerId;



            var customers = new CustomerService();
            var charges = new ChargeService();
            var amount = TempData["total"];
            var order = TempData["orderId"];
            var custt = TempData["cust"];
            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = 55,
                Description = "Sample Charge",
                Currency = "usd",
                CustomerId = customer.Id
            });

            Payments payment = new Payments();
            {
                payment.PaymentStripeId = charge.PaymentMethodId;
                payment.Amount = Convert.ToInt32(amount);
                payment.Paymentdate = System.DateTime.Now;
                payment.Cardno = Convert.ToInt32(charge.PaymentMethodDetails.Card.Last4);
                payment.OrderId = Convert.ToInt32(order);
                payment.CustomerId = Convert.ToInt32(custt);
            }
            //var customerService = new CustomerService();
            //ViewBag.details = charge.PaymentMethodDetails.Card.Last4;
            ViewBag.details = charge.PaymentMethodDetails.Card.GetType();
            context.Add<Payments>(payment);
            context.Payments.Add(payment);
            context.SaveChanges();

            return RedirectToAction("Invoice");

        }



        public IActionResult PaymentIndex()
        {
            var checkout = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.checkout = checkout;
            ViewBag.total = checkout.Sum(item => item.Products.ProductPrice * item.Quantity);

            return View();
        }

        public IActionResult Error()
        {
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
            // ViewBag.paymentId = int.Parse(TempData["paymentId"].ToString());
            foreach (var Item in cart)
            {
                Products p = context.Products.Find(Item.Products.ProductId);
                p.ProductQty = p.ProductQty - Item.Quantity;
                context.SaveChanges();
            }
            ViewBag.total = cart.Sum(item => item.Products.ProductPrice * item.Quantity);
            cart = null;
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            HttpContext.Session.Remove("cartitem");

            return View();
        }
        [Route("Plus")]
        [HttpGet]
        public IActionResult Plus(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            if (index != -1)
            {
                cart[index].Quantity++;
            }
            else
            {
                cart.Add(new Item
                {
                    Products = context.Products.Find(id),
                    Quantity = 1
                });

            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }
        [Route("Minus")]
        [HttpGet]
        public IActionResult Minus(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            int index = isExist(id);
            if (index != -1)
            {
                if (cart[index].Quantity != 1)
                {
                    cart[index].Quantity--;
                }

                else
                    return RedirectToAction("Remove", "Cart", new { @id = id });
            }
            SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        [Route("Search")]
        [HttpPost]
        public IActionResult Search(string search)
        {
            //if (search == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //var product = context.Products.Where(x => x.ProductName == Search || x.ProductCategory.CategoryName == Search
            // || x.Brand.BrandName == Search || Search == null).ToList();

            //return View(product);

            List<Products> prod = new List<Products>();
            var product = context.Products.Where(x => x.ProductName == search).ToList();
            if (context.Brands.Where(x => x.BrandName == search).SingleOrDefault() != null)
            {
                Brands b = context.Brands.Where(x => x.BrandName == search).SingleOrDefault();
                var brand = context.Products.Where(x => x.BrandId == b.BrandId).ToList();
                foreach (var item in brand)
                {
                    prod.Add(item);

                }
            }
            if (context.Categories.Where(x => x.CategoryName == search).SingleOrDefault() != null)
            {
                Categories c = context.Categories.Where(x => x.CategoryName == search).SingleOrDefault();
                var category = context.Products.Where(x => x.ProductCategoryId == c.ProductCategoryId).ToList();
                foreach (var item in category)
                {
                    prod.Add(item);

                }
            }

            foreach (var item in product)
            {
                prod.Add(item);

            }


            return View(prod);
        }

    }
}
