using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerMarketAPI.Common.Services
{
    public class BuyOneGetOneFreeRule : IDiscountRule
    {
        public bool IsApplicable(Dictionary<string, int> basket)
        {
            return basket.ContainsKey("CF1") && basket["CF1"] >= 2;
        }

        public decimal CalculateDiscount(Dictionary<string, int> basket, IProductRepository productRepository)
        {
            var product = productRepository.GetProductByCode("CF1");
            int coffeeQuantity = basket["CF1"];
            int freeCoffeeQuantity = coffeeQuantity / 2;
            decimal discountAmount = freeCoffeeQuantity * product.Price;
            return discountAmount;
        }
    }




}
