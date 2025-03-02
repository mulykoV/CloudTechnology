namespace Volt.Models
{
    public class ShopItem
    {
        public int Id { get; set; } 
        public Electronic electronic { get; set; }

        public decimal Price {  get; set; }
        
        public string ShopCartId { get; set; }
    }
}
