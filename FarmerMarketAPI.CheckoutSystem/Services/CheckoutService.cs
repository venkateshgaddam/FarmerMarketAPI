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

        public async Task<AddBasketResponse> AddToBasket(string productCode)
        {
            try
            {
                int _totalItems = 0;
                decimal _totalPrice = 0;
                decimal discountAmount = 0;
                List<string> _productCodes = new List<string>();
                if (productCode.Contains(','))
                {
                    _productCodes = productCode.Split(",").ToList();
                }
                else
                {
                    _productCodes.Add(productCode);
                }


                foreach (var product in _productCodes)
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

                if (_basketItemDetails.Any())
                {
                    foreach (var item in _basketItemDetails)
                    {
                        string _productCode = item.Key;
                        int quantity = item.Value;
                        var _product = productRepository.GetProductByCode(_productCode);
                        _totalItems += quantity;
                        if (_product != null)
                        {
                            _totalPrice += quantity * _product.Price;
                        }
                    }

                    discountAmount = await discountRuleEngine.CalculateDiscount(_basketItemDetails, productRepository);
                    _totalPrice -= discountAmount;
                }
                return new AddBasketResponse
                {
                    CartValue = _totalPrice,
                    TotalItems = _totalItems,
                    TotalDiscount = discountAmount
                };

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<decimal> CalculateTotalPrice(Dictionary<string, int> basketItemDetails)
        {
            decimal totalPrice = 0;

            foreach (var item in basketItemDetails)
            {
                string productCode = item.Key;
                int quantity = item.Value;
                var product = productRepository.GetProductByCode(productCode);

                if (product != null)
                {
                    totalPrice += quantity * product.Price;
                }
            }

            decimal discountAmount = await discountRuleEngine.CalculateDiscount(basketItemDetails, productRepository);
            totalPrice -= discountAmount;

            return totalPrice;
        }
         
    }
}
