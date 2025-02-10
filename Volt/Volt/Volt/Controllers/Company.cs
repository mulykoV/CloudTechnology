using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Volt.Data;
using Volt.Models;
using Volt.ViewModels;

namespace Volt.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public CompanyController(ApplicationDbContext dbContext)
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

            var company = _dbContext.Companies
                .Select(c => new CompanyViewModel(c))
                .ToList();

            return View(company);
        }

        [HttpGet]
        public IActionResult CreateCompany()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateCompany(Company company)
        {

            if (string.IsNullOrEmpty(company.PhotoCompany))
            {
                company.PhotoCompany = string.Empty;
            }

            if (company != null)
            {
                _dbContext.Companies.Add(company);
                _dbContext.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult DetailsCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _dbContext.Companies
                                    .Include(cm => cm.Electronics)
                                    .FirstOrDefault(b => b.CompanyId == id);

            if (company == null)
            {
                return NotFound();
            }

            var companyViewModel = new CompanyViewModel(company);

            return View(companyViewModel);
        }

        public IActionResult EditCompany(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _dbContext.Companies.Find(id);

            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        private bool CompanyExists(int id)
        {
            return _dbContext.Companies.Any(cm => cm.CompanyId == id);
        }

        [HttpPost]
        public IActionResult EditCompany(int id, Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dbContext.Update(company);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.CompanyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "Помилка при оновленні компанії. Будь ласка, спробуйте знову.");
                        return View(company);
                    }
                }
                return RedirectToAction("Index", "Home");
            }
            return View(company);
        }
    }
}
