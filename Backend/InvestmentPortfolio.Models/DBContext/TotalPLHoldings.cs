using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestmentPortfolio.Models.DBContext
{
    public class TotalPLHoldings
    {
        [Key]
        public int TProfitAndLossId { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalLoss { get; set; }
        public int InvestmentTypeId { get; set; }
    }
}
