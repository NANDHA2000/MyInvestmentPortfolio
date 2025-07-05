using InvestmentPortfolio.Models.MFPerformance;
using InvestmentPortfolio.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentPortfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MutualFundController : ControllerBase
    {

        private readonly IMutualFundService _mutualFundService;

        public MutualFundController(IMutualFundService mutualFundService)
        {
            _mutualFundService = mutualFundService;
        }

        [HttpGet("MutualFundHoldings")]
        public async Task<IActionResult> GetMFHoldings()
        {
            var holdings = await _mutualFundService.GetMFHoldings();
            return Ok(holdings);
        }

        [HttpPost("UploadGrowwReport")]
        public async Task<IActionResult> UploadGrowwReport(IFormFile file)
        {
            try
            {
                if(file == null || file.Length == 0)
                    return BadRequest("No file uploaded.");

                var result = await _mutualFundService.ProcessGrowwReportAsync(file);

                if(result.success)
                    return Ok(new { success = true, message = result.message });

                return BadRequest(new { success = false, message = result.message });
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("GetData")]
        public async Task<IActionResult> GetData([FromQuery] string schemeName, [FromQuery] string? fromDate, [FromQuery] string? toDate)
        {

            if(string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(toDate))
            {
                DateTime dateTime = DateTime.Now;
                fromDate = dateTime.AddDays(-30).ToString("yyyy-MM-dd");
                toDate = dateTime.ToString("yyyy-MM-dd");
            }
            // Path to the JSON file
            var jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "DayBydayMfPerformance.json");

            // Read the JSON content
            if(!System.IO.File.Exists(jsonFilePath))
            {
                return NotFound("JSON file not found.");
            }

            var jsonContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);

            // Deserialize JSON content into a list of lists
            var schemesList = System.Text.Json.JsonSerializer.Deserialize<List<List<Scheme>>>(jsonContent);

            if(schemesList == null || !schemesList.Any())
            {
                return NotFound("No data found in the JSON file.");
            }

            // Flatten the list if needed
            var schemes = schemesList.SelectMany(s => s).ToList();

            // Filter schemes based on schemeName if provided
            if(!string.IsNullOrEmpty(schemeName))
            {
                schemes = schemes.Where(s => s.SchemeName.Equals(schemeName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if(!schemes.Any())
            {
                return NotFound("No matching schemes found.");
            }

            // Filter SchemeReturns based on fromDate and toDate
            if(!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime from))
            {
                schemes.ForEach(s =>
                {
                    s.SchemeReturns = s.SchemeReturns.Where(r => DateTime.TryParse(r.Date, out DateTime date) && date >= from).ToList();
                });
            }

            if(!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime to))
            {
                schemes.ForEach(s =>
                {
                    s.SchemeReturns = s.SchemeReturns.Where(r => DateTime.TryParse(r.Date, out DateTime date) && date <= to).ToList();
                });
            }

            // If no SchemeReturns left after filtering, return an appropriate response
            if(!schemes.Any(s => s.SchemeReturns.Any()))
            {
                return NotFound("No data found for the specified date range.");
            }

            return Ok(schemes);
        }

    }
}
