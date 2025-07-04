using InvestmentPortfolio.Models.DBContext;
using InvestmentPortfolio.Models.Stocks;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentPortfolio.Service.IService
{
  public interface IStockService
  {
    Task<List<StockHoldings>> GetStocksHoldings();
    Task<TotalPLHoldings> GetProfitLossSummary();
    Task<(bool success, string message)> ProcessGrowwReportAsync(IFormFile file);
  }
}
