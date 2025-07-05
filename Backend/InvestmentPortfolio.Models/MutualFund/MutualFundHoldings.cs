namespace InvestmentPortfolio.Models.MutualFund
{
    public class MutualFundHoldings
    {
        public int MutualFundId { get; set; }
        public string? SchemeName { get; set; }
        public string? AMC { get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public string? Source { get; set; }
        public decimal Units { get; set; }
        public decimal InvestedValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal Returns { get; set; }
    }
}
