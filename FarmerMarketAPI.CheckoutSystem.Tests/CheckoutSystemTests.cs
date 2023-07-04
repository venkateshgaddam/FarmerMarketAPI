using FarmerMarketAPI.CheckoutSystem.Controllers;
using FarmerMarketAPI.Data.DbContext;
using FarmerMarketAPI.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using FarmerMarketAPI.Common.Interfaces;
using FarmerMarketAPI.CheckoutSystem.Services.Interface;
using FarmerMarketAPI.Data.Models;
using FarmerMarketAPI.Common.Services;
using FarmerMarketAPI.CheckoutSystem.Biz;
using FarmerMarketAPI.CheckoutSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace FarmerMarketAPI.CheckoutSystem.Tests
{
    [Trait("Category", "ControllerTests")]
    public class BasketControllerTests
    {
        private readonly BasketController _basketController;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IDiscountRuleEngine> _discountRuleEngineMock;
        private readonly Mock<BasketDbContext> _dbContextMock;
        private readonly Mock<IBasketBiz> _checkoutServiceMock;

        public BasketControllerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _discountRuleEngineMock = new Mock<IDiscountRuleEngine>();
            _dbContextMock = new Mock<BasketDbContext>();
            _checkoutServiceMock = new Mock<IBasketBiz>();

            _basketController = new BasketController(_checkoutServiceMock.Object);
        }

        [Fact]
        public void AddToBasket_ValidProductCode_ReturnsOk()
        {
            // Arrange
            string productCode = "CH1";

            // Act
            var result = _basketController.AddToBasket(new CheckoutRequest() { Products = productCode }).GetAwaiter().GetResult();

            // Assert
            Assert.IsType<JsonResult>(result);
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


    [Trait("Category", "CheckoutServiceTests")]
    public class CheckoutSystemTests
    {
        private Mock<IProductRepository> productRepository;
        private IDiscountRuleEngine discountRuleEngine;
        private ICheckoutService checkoutSystem;
        private readonly Mock<BasketDbContext> _dbContextMock;
        private List<Product> sampleProducts = new List<Product>
            {
                new Product { ProductCode = "CH1",Name="Chai",Price=3.11M },
                new Product { ProductCode = "AP1",Name="Apples",Price=6.00M },
                new Product { ProductCode = "CF1", Name = "Coffee", Price = 11.23M },
                new Product { ProductCode = "MK1", Name = "Milk", Price = 4.75M },
                new Product { ProductCode = "OM1", Name = "Oatmeal", Price = 3.69M }
            };

        public CheckoutSystemTests()
        {
            _dbContextMock = new Mock<BasketDbContext>();

            var mockDbSet = new Mock<DbSet<Product>>();

            // Configure the mock behavior
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(sampleProducts.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(sampleProducts.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(sampleProducts.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(sampleProducts.AsQueryable().GetEnumerator());

            _dbContextMock.Setup(c => c.BasketItems).Returns(mockDbSet.Object);
            productRepository = new Mock<IProductRepository>();// new ProductRepository(_dbContextMock.Object);// new Mock<IProductRepository>();
            productRepository.Setup(a => a.GetProductByCode(It.IsAny<string>())).Returns((string code) => sampleProducts.FirstOrDefault(a => a.ProductCode == code) ?? new Product());
            discountRuleEngine = new DiscountRuleEngine(new List<IDiscountRule>
            {
                new BuyOneGetOneFreeRule(),
                new ApplesDiscountRule(),
                new ChaiMilkDiscountRule(),
                new OatmealApplesDiscountRule()
            });
            checkoutSystem = new CheckoutService(productRepository.Object, discountRuleEngine);// CheckoutSystem(productRepository, discountRuleEngine);
        }
        

        [Fact]
        public void CalculateTotalPrice_NoDiscounts_ReturnsCorrectTotalPrice()
        {
            List<string> Products = new List<string> { "CH1", "AP1", "CF1", "MK1" };
            Dictionary<string, int> _basketItemDetails = AddtoBasket(Products);

            decimal totalPrice = (checkoutSystem.CalculateTotalPrice(_basketItemDetails)).GetAwaiter().GetResult().totalPrice;

            Assert.Equal(20.34m, totalPrice);
        }

        [Fact]
        public void CalculateTotalPrice_WithDiscounts_ReturnsCorrectTotalPrice()
        {
            List<string> Products = new List<string> { "AP1", "MK1" };

            var items = AddtoBasket(Products);

            decimal totalPrice = (checkoutSystem.CalculateTotalPrice(items)).GetAwaiter().GetResult().totalPrice;

            Assert.Equal(10.75m, totalPrice);
        }

        [Fact]
        public void CalculateTotalPrice_WithBuyOneGetOneFreeDiscount_ReturnsCorrectTotalPrice()
        {
            List<string> Products = new List<string> { "CF1", "CF1" };

            var items = AddtoBasket(Products);

            decimal totalPrice = (checkoutSystem.CalculateTotalPrice(items)).GetAwaiter().GetResult().totalPrice;

            Assert.Equal(11.23m, totalPrice);
        }

        [Fact]
        public void CalculateTotalPrice_WithApplesDiscount_ReturnsCorrectTotalPrice()
        {
            List<string> Products = new List<string> { "AP1", "AP1", "CH1", "AP1" };

            var items = AddtoBasket(Products);

            decimal totalPrice = (checkoutSystem.CalculateTotalPrice(items)).GetAwaiter().GetResult().totalPrice;

            Assert.Equal(16.61m, totalPrice);
        }


        private static Dictionary<string, int> AddtoBasket(List<string> Products)
        {
            Dictionary<string, int> _basketItemDetails = new Dictionary<string, int>();

            foreach (var product in Products)
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

            return _basketItemDetails;
        }

    }
}
