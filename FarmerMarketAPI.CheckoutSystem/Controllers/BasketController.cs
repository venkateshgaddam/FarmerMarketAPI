using FarmerMarketAPI.CheckoutSystem.Services.Interface;
using FarmerMarketAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace FarmerMarketAPI.CheckoutSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : BaseController
    {
        private readonly ICheckoutService checkoutService;

        public BasketController(ICheckoutService checkoutService)
        {
            this.checkoutService = checkoutService;
        }

        public ICheckoutService CheckoutService => checkoutService;

        [HttpPost("Add")]
        public async Task<IActionResult> AddToBasket([FromBody] CheckoutRequest checkoutRequest)
        {
            AddBasketResponse addBasketResponse = new AddBasketResponse();
            if (checkoutRequest.Products.Length >= 3)
            {
                addBasketResponse = await CheckoutService.AddToBasket(checkoutRequest.Products);
            }
            return PackageData(addBasketResponse, System.Net.HttpStatusCode.OK);

        }

        [HttpPost("TotalPrice")]
        public async Task<decimal> CalculateTotalPrice([FromBody] BasketInfo basketInfo)
        {
            Dictionary<string, int> keyValuePairs = new Dictionary<string, int>()
            {
                //{ "CH1",1 },
                {"AP1",1 },
                {"MK1",1 }
            };
            decimal totalPrice = await CheckoutService.CalculateTotalPrice(keyValuePairs);
            return totalPrice;
        }
    }
}
