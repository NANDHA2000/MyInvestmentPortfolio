using InvestmentPortfolio.Service.IService;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace InvestmentPortfolio.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class StocksController : ControllerBase
  {

    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
      _stockService = stockService;
    }


    [HttpGet("StocksHoldings")]
    public async Task<IActionResult> GetStocksHoldings()
    {
      var result = await _stockService.GetStocksHoldings();
      var totalProfitAndLoss = await _stockService.GetProfitLossSummary();

      if(result == null || !result.Any())
      {
        return NotFound("No stock holdings found.");
      }

      // Optional Debug Log
      Console.WriteLine("Stock Holdings Count: " + result.Count);

      var combinedObject = new
      {
        investmentDetails = result,
        TotalPL = totalProfitAndLoss
        // You can add more fields here later (profitLossSummary, filePeriod)
      };

      return Ok(combinedObject);
    }


    [HttpPost("UploadGrowwReport")]
    public async Task<IActionResult> UploadGrowwReport(IFormFile file)
    {
      try
      {
        if(file == null || file.Length == 0)
          return BadRequest("No file uploaded.");

        var result = await _stockService.ProcessGrowwReportAsync(file);

        if(result.success)
          return Ok(new { success = true, message = result.message });

        return BadRequest(new { success = false, message = result.message });
      }
      catch(Exception ex)
      {
        return StatusCode(500, new { success = false, message = ex.Message });
      }
    }
  }
}
