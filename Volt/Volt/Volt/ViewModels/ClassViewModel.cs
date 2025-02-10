using System.ComponentModel.DataAnnotations;
using Volt.Models;

namespace Volt.ViewModels
{
    public class ClassViewModel
    {
        public int ElectronicClassId { get; set; }

        public string Name { get; set; }

        public string About { get; set; }

        public ClassViewModel(ElectronicClass electronicClass) 
        {
            ElectronicClassId = electronicClass.ElectronicClassId;
            Name = electronicClass.Name;
            About = electronicClass.About;
        }
    }
}
