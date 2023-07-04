namespace FarmerMarketAPI.Data.Models
{
    public class AddBasketResponse
    {
        public decimal CartValue { get; set; }

        public int TotalItems { get; set; }

        public decimal TotalDiscount { get; set; }

        public string? CartDetails { get; set; }

    }
}
