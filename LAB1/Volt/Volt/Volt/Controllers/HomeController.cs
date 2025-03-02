using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using Volt.Data;
using Volt.Models;
using Volt.ViewModels;

namespace Volt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ShopCart _cart;


        public HomeController(ApplicationDbContext dbContext, ShopCart cart)
        {
            _dbContext = dbContext;
            _cart = cart;

        }

        public IActionResult Index()
        {
            var electronics = _dbContext.Electronics
                .Include(e => e.Class)
                .Include(e => e.Company)
                .Select(e => new ElectronicViewModel(e, e.Company, e.Class))
                .ToList();


            return View(electronics);
        }

        public IActionResult Options()
        {
            return View();
        }

        public ViewResult ShopCart()
        {
            var items = _cart.GetShopItems();
            _cart.listShopItems = items;

            var obj = new ShopCartViewModel
            {
                shopCart = _cart
            };

            return View(obj);
        }

        public RedirectToActionResult AddToCart(int id)
        {
            var item = _dbContext.Electronics.FirstOrDefault(i => i.ElectronicId == id);
            if (item != null) 
            {
                _cart.AddToCart(item);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Search(string searchString)
        {
            var electronics = _dbContext.Electronics
                 .Include(e => e.Class)
                 .Where(e => e.Model.Contains(searchString))
                 .Select(e => new ElectronicViewModel(e, e.Company, e.Class))
                  .ToList();

            return View("Index", electronics);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

