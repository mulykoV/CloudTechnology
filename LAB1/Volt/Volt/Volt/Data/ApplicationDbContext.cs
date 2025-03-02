using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Volt.Models;

namespace Volt.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Electronic> Electronics { get; set; }

        public DbSet<Purchase> Purchases { get; set; }

        public DbSet<ElectronicClass> ElectronicClasses { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<ShopItem> ShopItems { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Electronic>()
                .HasOne(e => e.Class)
                .WithMany(c => c.Electronics)
                .HasForeignKey(e => e.ElectronicClassId);

            builder.Entity<Electronic>()
                .HasOne(e => e.Company)
                .WithMany(cm => cm.Electronics)
                .HasForeignKey(e => e.CompanyId);

            builder.Entity<Electronic>()
                .Property(e => e.Price)
                .HasColumnType("decimal(18, 2)");
        }
    }
}