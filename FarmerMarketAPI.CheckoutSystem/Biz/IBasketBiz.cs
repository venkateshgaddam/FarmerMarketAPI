using FarmerMarketAPI.CheckoutSystem.Services.Interface;
using FarmerMarketAPI.Data.Models;
using FarmerMarketAPI.Data.Repository;

namespace FarmerMarketAPI.CheckoutSystem.Biz
{
    public interface IBasketBiz
    {
        Task<AddBasketResponse> AddItemsToBasket(string products);

        Task<decimal> CalculateTotalPrice(Dictionary<string, int> _basketItems);

    }

    public class BasketBiz : IBasketBiz
    {
        private readonly ICheckoutService _checkoutService;

        private readonly IProductRepository _productRepository;

        public BasketBiz(ICheckoutService checkoutService, IProductRepository productRepository)
        {
            _checkoutService = checkoutService;
            _productRepository = productRepository;
        }

        public async Task<AddBasketResponse> AddItemsToBasket(string products)
        {
            List<string> basketItems = new List<string>();
            string outputString = "There are ";
            decimal _totalPrice = 0;
            decimal _totalDiscount = 0;
            if (products.Contains(','))
            {
                basketItems = products.Split(",").ToList();
            }
            else
            {
                basketItems.Add(products);
            }

            var _basketItems = await _checkoutService.AddToBasket(basketItems);

            if (_basketItems.Any())
            {
                var result = await _checkoutService.CalculateTotalPrice(_basketItems);
                _totalPrice = result.totalPrice;
                _totalDiscount = result.totalDiscount;
            }

            var totalItems = _basketItems.Sum(a => a.Value);


            foreach (var item in _basketItems.Keys)
            {
                var count = _basketItems.Where(a => a.Key == item).Sum(a => a.Value);
                var product = _productRepository.GetProductByCode(item);

                outputString += $"{count} {product.Name} ,";
            }

            outputString = outputString.TrimEnd(',');
            outputString += " in the basket.";

            return new AddBasketResponse
            {
                CartValue = _totalPrice,
                TotalItems = totalItems,
                CartDetails = outputString,
                TotalDiscount = _totalDiscount,
            };

        }

        public async Task<decimal> CalculateTotalPrice(Dictionary<string, int> _basketItems)
        {
            var result = await _checkoutService.CalculateTotalPrice(_basketItems);
            return result.totalPrice;
        }
    }
}
