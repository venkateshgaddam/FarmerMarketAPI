using FarmerMarketAPI.Data.Models;

namespace FarmerMarketAPI.CheckoutSystem.Services.Interface
{
    public interface ICheckoutService
    {
        Task<(decimal totalPrice, decimal totalDiscount)> CalculateTotalPrice(Dictionary<string, int> basketItemDetails);

        Task<Dictionary<string, int>> AddToBasket(List<string> productCode);

    }
}
