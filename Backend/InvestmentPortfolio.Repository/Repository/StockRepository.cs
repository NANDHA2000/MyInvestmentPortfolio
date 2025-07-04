using Dapper;
using InvestmentPortfolio.Framework.Helper;
using InvestmentPortfolio.Models.DBContext;
using InvestmentPortfolio.Models.Models;
using InvestmentPortfolio.Models.Stocks;
using InvestmentPortfolio.Repository.DBContext;
using InvestmentPortfolio.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;


namespace InvestmentPortfolio.Repository.Repository
{
  public class StockRepository : IStockRepository
  {
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _context;

    public StockRepository(IConfiguration configuration, AppDbContext context)
    {
      _configuration = configuration;
      _context = context;
    }

    public async Task<List<StockHoldings>> GetStocksHoldings()
    {
      using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
      await connection.OpenAsync();

      // Execute the stored procedure
      var data = await connection.QueryAsync<StockHoldings>(
         "IP_GetStockHoldings",
         commandType: CommandType.StoredProcedure
     );

      return data.ToList();

    }

    public async Task<TotalPLHoldings> GetProfitLossSummary()
    {

      var totalProfit = await _context.TotalPLHoldings.SumAsync(p => p.TotalProfit);
      var totalLoss = await _context.TotalPLHoldings.SumAsync(p => p.TotalLoss);

      return new TotalPLHoldings
      {
        TotalProfit = totalProfit,
        TotalLoss = totalLoss
      };
    }

    public async Task SaveStockDataAsync(dynamic stockData)
    {
      using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
      await connection.OpenAsync();
      using var transaction = await connection.BeginTransactionAsync();

      try
      {
        // Deserialize JSON to strongly-typed object
        var data = JsonSerializer.Deserialize<Stocks>(JsonSerializer.Serialize(stockData));

        // Validate Holdings data
        if(data?.Holdings == null || data.Holdings.Count == 0)
        {
          throw new ArgumentException("No holdings data provided.");
        }

        foreach(var holding in data.Holdings)
        {
          await connection.ExecuteAsync(
              "IP_InsertStockHolding",
              new
              {
                StockName = holding.StockName,
                Quantity = ConversionHelper.ConvertToInt(holding.Quantity),
                PurchaseDate = ConversionHelper.ParseNullableDate(holding.BuyDate),
                PurchasePrice = ConversionHelper.ConvertToDecimal(holding.BuyPrice),
                PurchaseValue = ConversionHelper.ConvertToDecimal(holding.BuyValue),
                SellDate = ConversionHelper.ParseNullableDate(holding.SellDate),
                SellPrice = ConversionHelper.ConvertToDecimal(holding.SellPrice),
                SellValue = ConversionHelper.ConvertToDecimal(holding.SellValue),
                BookedProfitLoss = ConversionHelper.ConvertToDecimal(holding.RealisedPL)
              },
              transaction,
              commandType: CommandType.StoredProcedure
          );
        }

        await transaction.CommitAsync();
      }
      catch(Exception ex)
      {
        await transaction.RollbackAsync();
        Console.WriteLine($"Error: {ex.Message}");
        throw;
      }
    }


  }
}
