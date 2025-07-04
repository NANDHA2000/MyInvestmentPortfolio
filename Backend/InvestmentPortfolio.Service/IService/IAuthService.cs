using InvestmentPortfolio.Models.Models;

namespace InvestmentPortfolio.Service.IService
{
    public interface IAuthService
    {

        Task<bool> ValidateUser(string email, string password);
        Task<bool> RegisterUser(User user);
        Task<string> GetNavBar();
    }
}
