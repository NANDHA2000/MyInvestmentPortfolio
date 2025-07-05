using InvestmentPortfolio.Service.IService;
using InvestmentPortfolio.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentPortfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpGet("Features")]
        public async Task<IActionResult> GetFeatures()
        {
            var result = await _authService.GetFeatures();
            return Ok(result);

        }
    }
}
