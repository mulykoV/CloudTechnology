using Volt.Models;

namespace Volt.ViewModels
{
    public class ElectronicViewModel
    {
        public int ElectronicId { get; set; }

        public string? Class { get; set; }
        public int ClassID { get; set; }

        public string? Company { get; set; }
        public int CompanyID { get; set; }

        public string Model { get; set; }
        public decimal Price { get; set; }
        public string Characteristic { get; set; }

        public string PhotoOfElectronic { get; set; }

        public ElectronicViewModel(Electronic electronic, Company company, ElectronicClass clas)
        {
            ElectronicId = electronic.ElectronicId;
            CompanyID = electronic.CompanyId;
            ClassID = electronic.ElectronicClassId;
            Model = electronic.Model;
            Price = electronic.Price;
            Characteristic = electronic.Characteristic;
            PhotoOfElectronic = electronic.PhotoOfElectronic;


            Class = clas.Name;
            Company = company.CompanyName;
        }
    }
}
