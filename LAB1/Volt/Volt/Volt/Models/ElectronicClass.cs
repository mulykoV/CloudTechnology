using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Volt.Models
{
    public class ElectronicClass
    {
        [Key]
        public int ElectronicClassId { get; set; }

        [Required(ErrorMessage = "Поле Name є обов'язковим")]
        [StringLength(100, ErrorMessage = "Name повинно мати довжину не більше {1} символів")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Поле About є обов'язковим")]
        public string About { get; set; }

        public List<Electronic> Electronics { get; set; } = new List<Electronic>();
    }
}
