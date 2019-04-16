using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreEcommerceUserPanal.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreEcommerceUserPanal.Controllers
{
    public class ProductCategoryController : Controller
    {
        //ShoppingProjectContext context = new ShoppingProjectContext();
        private readonly ShoppingProjectContext _context;

        public ProductCategoryController(ShoppingProjectContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {

            var pc = _context.Categories.ToList();
            return View(pc);
        }
      
        public async Task<IActionResult> ProductDisplay(int? id)
        {
            var p = _context.Products.Where(x => x.ProductCategoryId == id).ToList();
            return View(p);
        }
       [HttpGet]
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var Categories = await _context.Categories.FindAsync(id);
            if (Categories == null)
            {
                return NotFound();
            }

            return Ok(Categories);
        }
    }
}