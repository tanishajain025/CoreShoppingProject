using CoreEcommerceUserPanal.Controllers;
using CoreEcommerceUserPanal.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace EcommerceUserPanal.Tests
{
   public class ProductCategoryTestController
    {
        private ShoppingProjectContext _context;

        public static DbContextOptions<ShoppingProjectContext>
            dbContextOptions
        { get; set; }

        public static string connectionString =
 "Data Source=TRD-519; Initial Catalog=ShoppingProject;Integrated Security=true;";

        static ProductCategoryTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<ShoppingProjectContext>()
                .UseSqlServer(connectionString).Options;
        }
        public ProductCategoryTestController()
        {
            _context = new ShoppingProjectContext(dbContextOptions);
        }
        [Fact]
        public async void Task_GetPcById_Return_OkResult()
        {
            var controller = new ProductCategoryController(_context);
            var PcId = 19;
            var data = await controller.ProductDisplay(PcId);
            Assert.IsType<ViewResult>(data);
        }
        [Fact]
        public async void Task_GetpcById_Return_FailResult()
        {
            var controller = new ProductCategoryController(_context);
            var PcId = 25;
            var data = await controller.Get(PcId);
            Assert.IsType<NotFoundResult>(data);
        }
        [Fact]
        public async void Task_GetUserById_MatchResult()
        {
            var controller = new ProductCategoryController(_context);
            int id = 1;
            var data = await controller.Get(id);
            Assert.IsType<OkObjectResult>(data);
            var OkResult = data.Should().BeOfType<OkObjectResult>().Subject;
            var user = OkResult.Value.Should().BeAssignableTo<Categories>().Subject;

            Assert.Equal("Top wear", user.CategoryName);
            Assert.Equal("Comfortable wear in every season", user.CategoryDescription);

        }
    }
}
