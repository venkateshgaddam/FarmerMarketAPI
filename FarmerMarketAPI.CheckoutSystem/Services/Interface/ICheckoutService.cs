using FarmerMarketAPI.Data.Models;

namespace FarmerMarketAPI.CheckoutSystem.Services.Interface
{
    public interface ICheckoutService
    {
        Task<decimal> CalculateTotalPrice(Dictionary<string, int> basketItemDetails);

        Task<AddBasketResponse> AddToBasket(string productCode);

    }
}
