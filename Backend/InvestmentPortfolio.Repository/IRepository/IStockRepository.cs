using InvestmentPortfolio.Models.DBContext;
using InvestmentPortfolio.Models.Stocks;



namespace InvestmentPortfolio.Repository.IRepository
{
  public interface IStockRepository
  {
    Task<List<StockHoldings>> GetStocksHoldings();
    Task<TotalPLHoldings> GetProfitLossSummary();
    Task SaveStockDataAsync(dynamic stockData);
  }
}
