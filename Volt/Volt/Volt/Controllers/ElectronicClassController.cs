using Microsoft.AspNetCore.Mvc;
using Volt.Models;
using Volt.Data;
using Microsoft.EntityFrameworkCore;
using Volt.ViewModels;

namespace Volt.Controllers
{
    public class ElectronicClassController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ElectronicClassController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var electronics = _dbContext.Electronics
                    .Include(e => e.Class)
                    .Include(e => e.Company)
                    .Select(e => new ElectronicViewModel(e, e.Company, e.Class))
                    .ToList();

            var classes = _dbContext.ElectronicClasses
                .Select(c => new ClassViewModel(c))
                .ToList();

            return View(classes);
        }

        [HttpGet]
        public IActionResult CreateElectronicClass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateElectronicClass(ElectronicClass electronicLass)
        {

            if (electronicLass != null)
            {
                _dbContext.ElectronicClasses.Add(electronicLass);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DetailsElectronicClass(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var electronicClass = _dbContext.ElectronicClasses
                .Include(cm => cm.Electronics)
                .FirstOrDefault(b => b.ElectronicClassId == id);

            if (electronicClass == null)
            {
                return NotFound();
            }

            var classViewModel = new ClassViewModel(electronicClass);

            return View(classViewModel);
        }

        public IActionResult EditElectronicClass(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clas = _dbContext.ElectronicClasses.Find(id);

            if (clas == null)
            {
                return NotFound();
            }

            return View(clas);
        }

        private bool ElectronicClassExists(int id)
        {
            return _dbContext.ElectronicClasses.Any(c => c.ElectronicClassId == id);
        }

        [HttpPost]
        public IActionResult EditElectronicClass(int id, ElectronicClass clas)
        {
            if (id != clas.ElectronicClassId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(clas);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ElectronicClassExists(clas.ElectronicClassId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Помилка при оновленні класу. Будь ласка, спробуйте знову.");
                        return View(clas);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(clas);
        }
    }
}
