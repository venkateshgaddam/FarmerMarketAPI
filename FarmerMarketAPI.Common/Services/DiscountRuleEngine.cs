using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Data.Repository;

namespace FarmerMarketAPI.Common.Services
{
    public class DiscountRuleEngine : IDiscountRuleEngine
    {
        private readonly List<IDiscountRule> discountRules;

        public DiscountRuleEngine(List<IDiscountRule> discountRules)
        {
            this.discountRules = discountRules;
        }

        public async Task<decimal> CalculateDiscount(Dictionary<string, int> basket, IProductRepository productRepository)
        {
            decimal discountAmount = 0;

            foreach (var rule in discountRules)
            {
                if (rule.IsApplicable(basket))
                {
                    discountAmount += rule.CalculateDiscount(basket, productRepository);
                }
            }

            return await Task.FromResult(discountAmount);
        }
    }


}
