using Dapper;
using InvestmentPortfolio.Framework.Helper;
using InvestmentPortfolio.Models.Models;
using InvestmentPortfolio.Models.MutualFund;
using InvestmentPortfolio.Repository.DBContext;
using InvestmentPortfolio.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;

namespace InvestmentPortfolio.Repository.Repository
{
    public class MutualFundRepository : IMutualFundRepository
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public MutualFundRepository(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<List<MutualFundHoldings>> GetMFHoldings()
        {
            using(var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var result = await connection.QueryAsync<MutualFundHoldings>(
                    "IP_GetMutualFundHoldings",
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
        }

        public async Task SaveMutualFundDataAsync(dynamic mutualFundData)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var data = JsonSerializer.Deserialize<MutualFund>(JsonSerializer.Serialize(mutualFundData));

                if(data.Holdings != null)
                {
                    foreach(var holding in data.Holdings)
                    {
                        if(holding.SchemeName == null) continue; // Skip empty records

                        await connection.ExecuteAsync(
                            "IP_InsertMutualFundHolding",
                            new
                            {
                                SchemeName = holding.SchemeName,
                                AMC = holding.AMC,
                                Category = holding.Category,
                                SubCategory = holding.SubCategory,
                                Source = holding.Source,
                                Units = ConversionHelper.ConvertToDecimal(holding.Units),
                                InvestedValue = ConversionHelper.ConvertToDecimal(holding.InvestedValue),
                                CurrentValue = ConversionHelper.ConvertToDecimal(holding.CurrentValue),
                                Returns = ConversionHelper.ConvertToDecimal(holding.Returns),
                                XIRR = ConversionHelper.ConvertToDecimal(holding.XIRR)
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure
                        );
                    }
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
