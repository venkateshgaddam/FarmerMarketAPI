using FarmerMarketAPI.Data.Repository;

namespace FarmerMarketAPI.Common.Interfaces
{
    public interface IDiscountRuleEngine
    {
        Task<decimal> CalculateDiscount(Dictionary<string, int> basket, IProductRepository productRepository);
    }
}
