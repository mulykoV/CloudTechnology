using Volt.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Volt.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Volt.Controllers
{
    public class ElectronicController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ElectronicController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateElectronic()
        {
            var electronicClass = _dbContext.ElectronicClasses.ToList();
            ViewBag.ElectronicLasses = new SelectList(electronicClass, "ElectronicClassId", "Name");

            var company = _dbContext.Companies.ToList();
            ViewBag.Companies = new SelectList(company, "CompanyId", "CompanyName");

            return View();
        }

        [HttpPost]
        public IActionResult CreateElectronic(Electronic electronic)
        {
            if (electronic != null)
            {

                _dbContext.Electronics.Add(electronic);

                _dbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DetailsElectronic(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var electronic = _dbContext.Electronics
                                    .Include(e => e.Class)
                                    .Include(e => e.Company)
                                    .FirstOrDefault(e => e.ElectronicId == id);

            if (electronic == null)
            {
                return NotFound();
            }

            return View(electronic);
        }

        [HttpGet]
        public IActionResult EditElectronic(int id)
        {
            var electronic = _dbContext.Electronics
                .Include(e => e.Class)
                .FirstOrDefault(e => e.ElectronicId == id);

            if (electronic == null)
            {
                return NotFound();
            }

            var electronicClass = _dbContext.ElectronicClasses.ToList();
            var company = _dbContext.Companies.ToList();

            ViewBag.ElectronicClasses = new SelectList(electronicClass, "ElectronicClassId", "Name");
            ViewBag.Companies = new SelectList(company, "CompanyId", "CompanyName");

            return View(electronic);
        }

        private bool ElectronicExists(int id)
        {
            return _dbContext.Electronics.Any(a => a.ElectronicId == id);
        }

        [HttpPost]
        public IActionResult EditElectronic(int id, Electronic updatedElectronic)
        {
            if (id != updatedElectronic.ElectronicId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingElectronic = _dbContext.Electronics.FirstOrDefault(e => e.ElectronicId == id);

                    if (existingElectronic == null)
                    {
                        return NotFound();
                    }

                    existingElectronic.Model = updatedElectronic.Model;
                    existingElectronic.Price = updatedElectronic.Price;
                    existingElectronic.Characteristic = updatedElectronic.Characteristic;
                    existingElectronic.PhotoOfElectronic = updatedElectronic.PhotoOfElectronic;
                    existingElectronic.ElectronicClassId = updatedElectronic.ElectronicClassId;
                    existingElectronic.CompanyId = updatedElectronic.CompanyId;

                    _dbContext.Update(existingElectronic);
                    _dbContext.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ElectronicExists(updatedElectronic.ElectronicId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Помилка при оновленні ґаджета. Будь ласка, спробуйте знову.");
                        return View(updatedElectronic);
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", $"Помилка при оновленні даних: {ex.Message}");
                    return View(updatedElectronic);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }

            var electronicClass = _dbContext.ElectronicClasses.ToList();
            var company = _dbContext.Companies.ToList();

            ViewBag.ElectronicClasses = new SelectList(electronicClass, "ElectronicClassId", "Name");
            ViewBag.Companies = new SelectList(company, "CompanyId", "CompanyName");

            return View(updatedElectronic);
        }



        public async Task<ActionResult> Buy(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var electronic = await _dbContext.Electronics.FirstOrDefaultAsync(e => e.ElectronicId == Id);

            if (electronic == null)
            {
                return NotFound();
            }

            return View(electronic);
        }

        [HttpPost]
        public string Buy(Purchase purchase)
        {
            purchase.Date = DateTime.Now;
            _dbContext.Purchases.Add(purchase);
            _dbContext.SaveChanges();

            return "Дякуємо, " + purchase.PurchaseName + ", за купівлю!";
        }

        [HttpGet]
        public IActionResult DeleteElectronic()
        {

            var electronic = _dbContext.Electronics.ToList();

            return View(electronic);
        }

        [HttpPost]
        public IActionResult DeleteElectronic(int id)
        {
            var electronic = _dbContext.Electronics.Find(id);

            if (electronic == null)
            {
                return NotFound();
            }

            _dbContext.Electronics.Remove(electronic);
            _dbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
    }
}
