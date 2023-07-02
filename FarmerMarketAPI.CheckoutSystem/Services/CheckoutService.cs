using FarmerMarketAPI.CheckoutSystem.Services.Interface;
using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.Data.Models;
using FarmerMarketAPI.Data.Repository;
using Microsoft.Extensions.Caching.Memory;

namespace FarmerMarketAPI.CheckoutSystem.Services
{

    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void AddItemToCache(string key, object value)
        {
            // Store the item in the cache
            _memoryCache.Set(key, value);
        }

        public T GetItemFromCache<T>(string key)
        {
            // Retrieve the item from the cache
            if (_memoryCache.TryGetValue<T>(key, out var value))
            {
                // Item found in the cache
                return value;
            }

            // Item not found in the cache
            return default;
        }

        public bool IsItemInCache(string key)
        {
            // Check if the item exists in the cache
            return _memoryCache.TryGetValue(key, out _);
        }

        public void RemoveItemFromCache(string key)
        {
            // Remove the item from the cache
            _memoryCache.Remove(key);
        }
    }

    public interface ICacheService
    {
        void AddItemToCache(string key, object value);
        void RemoveItemFromCache(string key);

        bool IsItemInCache(string key);

        T GetItemFromCache<T>(string key);
    }

    public class CheckoutService : ICheckoutService
    {
        private readonly IProductRepository productRepository;
        private readonly IDiscountRuleEngine discountRuleEngine;
        private readonly ICacheService _memoryCache;
        private Dictionary<string, int> _basketItemDetails=new Dictionary<string, int>();

        public CheckoutService(IProductRepository productRepository, IDiscountRuleEngine discountRuleEngine, ICacheService memoryCache)
        {
            this.productRepository = productRepository;
            this.discountRuleEngine = discountRuleEngine;
            _memoryCache = memoryCache;
        }

        public async Task<AddBasketResponse> AddToBasket(string productCode)
        {
            try
            {
                int _totalItems = 0;
                decimal _totalPrice = 0;
                if (_memoryCache.IsItemInCache(productCode))
                {
                    var _currentQuantity = _memoryCache.GetItemFromCache<int>(productCode);
                    _currentQuantity++;
                    _memoryCache.AddItemToCache(productCode, _currentQuantity);
                    _basketItemDetails.Add(productCode, _currentQuantity);
                }
                else
                {
                    _memoryCache.AddItemToCache(productCode, 1);
                    _basketItemDetails.Add(productCode, 1);
                }

                if (_basketItemDetails.Any())
                {
                    foreach (var item in _basketItemDetails)
                    {
                        string _productCode = item.Key;
                        int quantity = item.Value;
                        var product = productRepository.GetProductByCode(_productCode);
                        _totalItems += quantity;
                        if (product != null)
                        {
                            _totalPrice += quantity * product.Price;
                        }
                    }

                    decimal discountAmount = await discountRuleEngine.CalculateDiscount(_basketItemDetails, productRepository);
                    _totalPrice -= discountAmount;
                }
                return new AddBasketResponse
                {
                    CartValue = _totalPrice,
                    TotalItems = _totalItems,
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
