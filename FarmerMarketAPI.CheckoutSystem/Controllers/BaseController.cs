using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmerMarketAPI.CheckoutSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : Controller
    {
        protected JsonResult PackageData<T>(T data, System.Net.HttpStatusCode httpStatusCode)
        {
            var result = Json(data);
            result.StatusCode = (int)httpStatusCode;
            return result;
        }
    }
}
