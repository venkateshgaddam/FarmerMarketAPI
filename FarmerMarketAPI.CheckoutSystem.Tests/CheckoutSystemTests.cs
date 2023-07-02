using FarmerMarketAPI.CheckoutSystem.Controllers;
using FarmerMarketAPI.Data.DbContext;
using FarmerMarketAPI.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.CheckoutSystem.Services.Interface;
using FarmerMarketAPI.Data.Models;

namespace FarmerMarketAPI.CheckoutSystem.Tests
{
    public class CheckoutSystemTests
    {
    }

    public class BasketControllerTests
    {
        private readonly BasketController _basketController;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IDiscountRuleEngine> _discountRuleEngineMock;
        private readonly Mock<BasketDbContext> _dbContextMock;
        private readonly Mock<ICheckoutService> _checkoutServiceMock;

        public BasketControllerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _discountRuleEngineMock = new Mock<IDiscountRuleEngine>();
            _dbContextMock = new Mock<BasketDbContext>();
            _checkoutServiceMock = new Mock<ICheckoutService>();
            _basketController = new BasketController(_checkoutServiceMock.Object);
        }

        [Fact]
        public void AddToBasket_ValidProductCode_ReturnsOk()
        {
            // Arrange
            string productCode = "CH1";

            // Act
            var result = _basketController.AddToBasket(productCode);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async void CalculateTotalPrice_ValidBasketInfo_ReturnsTotalPrice()
        {
            // Arrange
            var basketInfo = new BasketInfo();
            // Mock the behavior of the checkoutSystem dependency
            decimal expectedTotalPrice = 100m;
            //Mock<CheckoutSystem> checkoutSystemMock = new Mock<CheckoutSystem>(
            //    _productRepositoryMock.Object,
            //    _discountRuleEngineMock.Object
            //);
            _checkoutServiceMock.Setup(cs => cs.CalculateTotalPrice(It.IsAny<Dictionary<string, int>>())).Returns(Task.FromResult(expectedTotalPrice));

            //_basketController.SetCheckoutSystem(checkoutSystemMock.Object);

            // Act
            var result = await _basketController.CalculateTotalPrice(basketInfo);

            // Assert
            Assert.Equal(expectedTotalPrice, result);
        }
    }


    //[TestFixture]
    //public class CheckoutSystemTests
    //{
    //    private IProductRepository productRepository;
    //    private IDiscountRuleEngine discountRuleEngine;
    //    private CheckoutSystem checkoutSystem;

    //    [SetUp]
    //    public void Setup()
    //    {
    //        productRepository = new ProductRepository();
    //        discountRuleEngine = new DiscountRuleEngine(new List<IDiscountRule>
    //    {
    //        new BuyOneGetOneFreeRule(),
    //        new ApplesDiscountRule(),
    //        new ChaiMilkDiscountRule(),
    //        new OatmealApplesDiscountRule()
    //    });
    //        checkoutSystem = new CheckoutSystem(productRepository, discountRuleEngine);
    //    }

    //    [Test]
    //    public void CalculateTotalPrice_NoDiscounts_ReturnsCorrectTotalPrice()
    //    {
    //        checkoutSystem.AddToBasket("CH1");
    //        checkoutSystem.AddToBasket("AP1");
    //        checkoutSystem.AddToBasket("CF1");
    //        checkoutSystem.AddToBasket("MK1");

    //        decimal totalPrice = checkoutSystem.CalculateTotalPrice();

    //        Assert.AreEqual(20.34m, totalPrice);
    //    }

    //    [Test]
    //    public void CalculateTotalPrice_WithDiscounts_ReturnsCorrectTotalPrice()
    //    {
    //        checkoutSystem.AddToBasket("MK1");
    //        checkoutSystem.AddToBasket("AP1");

    //        decimal totalPrice = checkoutSystem.CalculateTotalPrice();

    //        Assert.AreEqual(10.75m, totalPrice);
    //    }

    //    [Test]
    //    public void CalculateTotalPrice_WithBuyOneGetOneFreeDiscount_ReturnsCorrectTotalPrice()
    //    {
    //        checkoutSystem.AddToBasket("CF1");
    //        checkoutSystem.AddToBasket("CF1");

    //        decimal totalPrice = checkoutSystem.CalculateTotalPrice();

    //        Assert.AreEqual(11.23m, totalPrice);
    //    }

    //    [Test]
    //    public void CalculateTotalPrice_WithApplesDiscount_ReturnsCorrectTotalPrice()
    //    {
    //        checkoutSystem.AddToBasket("AP1");
    //        checkoutSystem.AddToBasket("AP1");
    //        checkoutSystem.AddToBasket("CH1");
    //        checkoutSystem.AddToBasket("AP1");

    //        decimal totalPrice = checkoutSystem.CalculateTotalPrice();

    //        Assert.AreEqual(16.61m, totalPrice);
    //    }
    //}
}
