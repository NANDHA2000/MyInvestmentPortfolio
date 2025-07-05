using InvestmentPortfolio.Models.MutualFund;

namespace InvestmentPortfolio.Repository.IRepository
{
    public interface IMutualFundRepository
    {
        Task<List<MutualFundHoldings>> GetMFHoldings();
        Task SaveMutualFundDataAsync(dynamic mutualFundResult);
    }
}
