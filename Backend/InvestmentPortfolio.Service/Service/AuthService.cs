using InvestmentPortfolio.Models.Models;
using InvestmentPortfolio.Repository.IRepository;
using InvestmentPortfolio.Service.IService;


namespace InvestmentPortfolio.Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<bool> ValidateUser(string email, string password)
        {
            return await _authRepository.ValidateUser(email, password);
        }


        public async Task<bool> RegisterUser(User user)
        {
            return await _authRepository.RegisterUser(user);
        }

        public async Task<List<dynamic>> GetFeatures()
        {
            return await _authRepository.GetFeatures();
        }
    }
}
