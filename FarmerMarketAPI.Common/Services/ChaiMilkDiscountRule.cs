using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerMarketAPI.Common.Services
{
    public class ChaiMilkDiscountRule : IDiscountRule
    {
        public bool IsApplicable(Dictionary<string, int> basket)
        {
            return basket.ContainsKey("CH1") && basket.ContainsKey("MK1") && basket["CH1"] >= 1 && basket["MK1"] >= 1;
        }

        public decimal CalculateDiscount(Dictionary<string, int> basket, IProductRepository productRepository)
        {
            var product = productRepository.GetProductByCode("MK1");
            decimal discountAmount = product.Price;
            return discountAmount;
        }
    }
}
