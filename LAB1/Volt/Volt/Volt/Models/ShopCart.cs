using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Volt.Data;

namespace Volt.Models
{
    public class ShopCart
    {
        public readonly ApplicationDbContext _dbContext;
        public ShopCart(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        
        }
        
        [Key]
        public string ShopCartId { get; set; }

        public List<ShopItem> listShopItems { get; set; }

        public static ShopCart GetCart(IServiceProvider services)
        {
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;
            var context = services.GetService<ApplicationDbContext>();
            string shopCartId = session.GetString("CartId") ?? Guid.NewGuid().ToString();

            session.SetString("CartId", shopCartId);

            return new ShopCart(context) { ShopCartId = shopCartId };
        }

        public void AddToCart(Electronic electronic)
        {
            this._dbContext.ShopItems.Add(new ShopItem
            {
                ShopCartId = ShopCartId,
                electronic = electronic,
                Price = electronic.Price,
            });

            _dbContext.SaveChanges();
        }

        public List<ShopItem> GetShopItems()
        {
            return _dbContext.ShopItems.Where(c => c.ShopCartId == ShopCartId)
                .Include(s => s.electronic)
                .ToList();
        }
    
    }

}
