using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentPortfolio.Models.Stocks
{
    public class StockHoldings
    {
        public string? StockName { get; set; }
        public long? Quantity { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? PurchaseValue { get; set; }
        public DateTime? SellDate { get; set; }
        public decimal? SellPrice { get; set; }
        public decimal? SellValue { get; set; }
        public decimal? BookedProfitLoss { get; set; }
    }
}
