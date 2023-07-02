using FarmerMarketAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FarmerMarketAPI.Data.DbContext
{
    public class BasketDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<Product> BasketItems { get; set; }

        public BasketDbContext(DbContextOptions<BasketDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasKey(p => p.ProductCode);
        }

    }
}
