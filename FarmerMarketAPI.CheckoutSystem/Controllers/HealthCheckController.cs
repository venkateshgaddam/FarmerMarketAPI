using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FarmerMarketAPI.CheckoutSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("ping")]
        public IActionResult Ping()
        {
            return Ok(
                new Dictionary<string, object>
                {
                    {"status", "Success"},
                    {"AssemblyVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString()},
                }
            );
        }
    }
}
