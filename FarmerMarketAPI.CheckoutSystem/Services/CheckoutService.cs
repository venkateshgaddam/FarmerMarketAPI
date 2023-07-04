using FarmerMarketAPI.CheckoutSystem.Services.Interface;
using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Data.Models;
using FarmerMarketAPI.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace FarmerMarketAPI.CheckoutSystem.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IProductRepository productRepository;
        private readonly IDiscountRuleEngine discountRuleEngine;
        private Dictionary<string, int> _basketItemDetails = new Dictionary<string, int>();

        public CheckoutService(IProductRepository productRepository, IDiscountRuleEngine discountRuleEngine)
        {
            this.productRepository = productRepository;
            this.discountRuleEngine = discountRuleEngine;
        }

        public async Task<Dictionary<string, int>> AddToBasket(List<string> productCode)
        {
            try
            {
                foreach (var product in productCode)
                {
                    if (_basketItemDetails.ContainsKey(product))
                    {
                        var _currentQuantity = _basketItemDetails[product];
                        _currentQuantity++;
                        _basketItemDetails[product] = _currentQuantity;
                    }
                    else
                    {
                        _basketItemDetails.Add(product, 1);
                    }
                }

                return await Task.FromResult(_basketItemDetails);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<(decimal totalPrice, decimal totalDiscount)> CalculateTotalPrice(Dictionary<string, int> basketItemDetails)
        {
            decimal totalPrice = 0;
            int _totalItems = 0;
            foreach (var item in basketItemDetails)
            {
                string productCode = item.Key;
                int quantity = item.Value;
                var product = productRepository.GetProductByCode(productCode);
                _totalItems += quantity;
                if (product != null)
                {
                    totalPrice += quantity * product.Price;
                }
            }

            decimal totalDiscount = await discountRuleEngine.CalculateDiscount(basketItemDetails, productRepository);
            totalPrice -= totalDiscount;

            return (totalPrice, totalDiscount);
        }

    }
}
