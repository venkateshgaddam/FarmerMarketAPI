using FarmerMarketAPI.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmerMarketAPI.CheckoutSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : BaseController
    {
        private readonly IProductRepository productRepository;
        public ProductsController(IProductRepository productRepository)
        {
            this.productRepository = productRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await productRepository.GetProducts();
            return PackageData(result,System.Net.HttpStatusCode.OK);
        }
    }
}
