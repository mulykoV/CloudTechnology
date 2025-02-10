namespace Volt.Models
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string AboutCompany { get; set; }

        public string CountryOfCompany { get; set; }

        public string PhoneNumber { get; set; }

        public string PhotoCompany { get; set; }

        public List<Electronic> Electronics { get; set; } = new List<Electronic>();
    }
}
