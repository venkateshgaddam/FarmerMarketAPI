using FarmerMarketAPI.Data.DbContext;
using FarmerMarketAPI.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerMarketAPI.Data.Repository
{
    public class ProductRepository : IProductRepository
    {

        private readonly BasketDbContext dbContext;

        public ProductRepository(BasketDbContext dbContext)
        {
            this.dbContext = dbContext;
            SeedData(dbContext);

        }

        public async Task<List<Product>> GetProducts()
        {
            var result = dbContext.BasketItems.ToList();

            return await Task.FromResult(result);
        }


        public Product GetProductByCode(string productCode)
        {
            var dataItems = dbContext.BasketItems.ToList();
            var result = dataItems.FirstOrDefault(p => p.ProductCode.ToLower() == productCode.ToLower());
            return result;
        }
        private static void SeedData(BasketDbContext dbContext)
        {
            if (!dbContext.BasketItems.Any())
            {
                var initialBasketItems = new List<Product>
        {
            new Product { ProductCode = "CH1",Name="Chai",Price=3.11M },
            new Product { ProductCode = "AP1",Name="Apples",Price=6.00M },
            new Product { ProductCode = "CF1",Name="Coffee",Price=11.23M },
            new Product { ProductCode = "MK1",Name="Milk",Price=4.75M },
            new Product { ProductCode = "OM1",Name="Oatmeal",Price=3.69M }
        };

                dbContext.BasketItems.AddRange(initialBasketItems);
                dbContext.SaveChanges();
            }
        }
    }
}
