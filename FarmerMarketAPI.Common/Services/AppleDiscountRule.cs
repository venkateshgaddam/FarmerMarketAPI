using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerMarketAPI.Common.Services
{

    public class ApplesDiscountRule : IDiscountRule
    {
        public bool IsApplicable(Dictionary<string, int> basket)
        {
            return basket.ContainsKey("AP1") && basket["AP1"] >= 3;
        }

        public decimal CalculateDiscount(Dictionary<string, int> basket, IProductRepository productRepository)
        {
            var product = productRepository.GetProductByCode("AP1");
            int applesQuantity = basket["AP1"];
            decimal discountAmount = (applesQuantity) * (product.Price - 4.50m);
            return discountAmount;
        }
    }

    

   

}
