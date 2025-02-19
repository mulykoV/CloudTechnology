using System.ComponentModel.DataAnnotations.Schema;

namespace Volt.Models
{
    public class Electronic
    {
        public int ElectronicId { get; set; }

        public int ElectronicClassId { get; set; }
        public ElectronicClass? Class { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        
        public string Model { get; set; }
        public decimal Price { get; set; }
        public string Characteristic { get; set; }

        public string PhotoOfElectronic { get; set; }
    }
}
