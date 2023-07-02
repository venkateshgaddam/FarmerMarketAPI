using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Data.Repository;

namespace FarmerMarketAPI.Common.Services
{
    public class OatmealApplesDiscountRule : IDiscountRule
    {
        public bool IsApplicable(Dictionary<string, int> basket)
        {
            return basket.ContainsKey("OM1") && basket.ContainsKey("AP1");
        }

        public decimal CalculateDiscount(Dictionary<string, int> basket, IProductRepository productRepository)
        {
            var product = productRepository.GetProductByCode("AP1");
            int oatmealQuantity = basket["OM1"];
            int applesQuantity = basket["AP1"];
            decimal discountAmount = Math.Min(oatmealQuantity, applesQuantity) * (product.Price * 0.5m);
            return discountAmount;
        }
    }

}
