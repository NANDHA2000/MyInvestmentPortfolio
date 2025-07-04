using ExcelDataReader;
using InvestmentPortfolio.Models.DBContext;
using InvestmentPortfolio.Models.Stocks;
using InvestmentPortfolio.Repository.IRepository;
using InvestmentPortfolio.Service.IService;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;

namespace InvestmentPortfolio.Service.Service
{
  public class StockService : IStockService
  {

    private readonly IStockRepository _stockRepository;

    public StockService(IStockRepository stockRepository)
    {
      _stockRepository = stockRepository;
    }


    public async Task<List<StockHoldings>> GetStocksHoldings()
    {
      return await _stockRepository.GetStocksHoldings();
    }

    public Task<TotalPLHoldings> GetProfitLossSummary()
    {
      return _stockRepository.GetProfitLossSummary();
    }

    public async Task<(bool success, string message)> ProcessGrowwReportAsync(IFormFile file)
    {
      try
      {
        var filePath = Path.GetTempFileName();
        using(var stream = new FileStream(filePath, FileMode.Create))
        {
          await file.CopyToAsync(stream);
        }

        var dataTable = ReadGrowwReport(filePath);

        var stockResult = ProcessGrowwStockData(dataTable);

        await _stockRepository.SaveStockDataAsync(stockResult);

        return (true, "File Data Added");
      }
      catch(Exception ex)
      {
        throw new Exception($"Error processing file: {ex.Message}");
      }
    }


    #region Stocks
    private object ProcessGrowwStockData(DataTable table)
    {
      var report = new
      {
        PersonalDetails = new
        {
          Name = table.Rows[0]["Column1"]?.ToString(),
          UniqueClientCode = table.Rows[1]["Column1"]?.ToString(),
          PLStatement = table.Rows[3]["Column0"]?.ToString(),
        },
        ProfitAndLoss = new
        {
          RealisedPL = table.Rows[8]["Column1"]?.ToString(),
          UnRealisedPL = table.Rows[9]["Column1"]?.ToString(),
        },
        Charges = new
        {
          ExchangeTransactionCharges = table.Rows[12]["Column1"]?.ToString(),
          SEBICharges = table.Rows[13]["Column1"]?.ToString(),
          STT = table.Rows[14]["Column1"]?.ToString(),
          StampDuty = table.Rows[15]["Column1"]?.ToString(),
          IPFTCharges = table.Rows[16]["Column1"]?.ToString(),
          Brokerage = table.Rows[17]["Column1"]?.ToString(),
          DPCharges = table.Rows[18]["Column1"]?.ToString(),
          TotalGST = table.Rows[19]["Column1"]?.ToString(),
          Total = table.Rows[20]["Column1"]?.ToString(),
        },
        Holdings = GetStockHoldings(table)
      };

      return report;
    }

    private List<object> GetStockHoldings(DataTable table)
    {
      var holdings = new List<object>();

      // Adjust the starting index based on where holdings start
      for(int i = 25; i < table.Rows.Count; i++)
      {
        var row = table.Rows[i];
        // Check if the row contains valid data for stock holdings
        if(string.IsNullOrWhiteSpace(row["Column0"]?.ToString()) ||
            row["Column0"]?.ToString()!.Contains("Disclaimer", StringComparison.OrdinalIgnoreCase) == true ||
            row["Column0"]?.ToString()!.Contains("This report is provided", StringComparison.OrdinalIgnoreCase) == true ||
            row["Column0"]?.ToString()!.Contains("Groww Invest Tech Private Limited", StringComparison.OrdinalIgnoreCase) == true)
        {
          // Skip unwanted or disclaimer rows
          continue;
        }

        var holding = new Dictionary<string, object>();

        // Add non-empty fields to the dictionary
        if(!string.IsNullOrWhiteSpace(row["Column0"]?.ToString()))
          holding["StockName"] = row["Column0"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column1"]?.ToString()))
          holding["ISIN"] = row["Column1"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column2"]?.ToString()))
          holding["Quantity"] = row["Column2"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column3"]?.ToString()))
          holding["BuyDate"] = row["Column3"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column4"]?.ToString()))
          holding["BuyPrice"] = row["Column4"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column5"]?.ToString()))
          holding["BuyValue"] = row["Column5"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column6"]?.ToString()))
          holding["SellDate"] = row["Column6"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column7"]?.ToString()))
          holding["SellPrice"] = row["Column7"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column8"]?.ToString()))
          holding["SellValue"] = row["Column8"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column9"]?.ToString()))
          holding["RealisedPL"] = row["Column9"]?.ToString()!;
        if(!string.IsNullOrWhiteSpace(row["Column10"]?.ToString()))
          holding["Remarks"] = row["Column10"]?.ToString()!;

        holdings.Add(holding);
      }

      return holdings;
    }
    #endregion

    #region Read Excel file
    private DataTable ReadGrowwReport(string filePath)
    {
      try
      {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var result = reader.AsDataSet();
        if(result.Tables.Count == 0)
          throw new Exception("The uploaded file does not contain any data.");

        return result.Tables[0];
      }
      catch(Exception ex)
      {
        throw new Exception($"Failed to read the Excel file: {ex.Message}");
      }
    }
    #endregion
  }
}
