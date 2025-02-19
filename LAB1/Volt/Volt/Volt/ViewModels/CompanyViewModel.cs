using Volt.Models;
namespace Volt.ViewModels
{
    public class CompanyViewModel
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string AboutCompany { get; set; }

        public string CountryOfCompany { get; set; }

        public string PhoneNumber { get; set; }

        public string PhotoCompany { get; set; }

        public CompanyViewModel(Company company)
        {
            CompanyId = company.CompanyId;
            CompanyName = company.CompanyName;
            AboutCompany = company.AboutCompany;
            CountryOfCompany = company.CountryOfCompany;
            PhoneNumber = company.PhoneNumber;
            PhotoCompany = company.PhotoCompany;
        }
    }
}
