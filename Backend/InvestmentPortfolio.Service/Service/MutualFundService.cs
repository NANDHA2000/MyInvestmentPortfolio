using ExcelDataReader;
using InvestmentPortfolio.Models.MutualFund;
using InvestmentPortfolio.Repository.IRepository;
using InvestmentPortfolio.Repository.Repository;
using InvestmentPortfolio.Service.IService;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text;

namespace InvestmentPortfolio.Service.Service
{
    public class MutualFundService:IMutualFundService
    {
        private readonly IMutualFundRepository _mutualFundRepository;

        public MutualFundService(IMutualFundRepository mutualFundRepository)
        {
            _mutualFundRepository = mutualFundRepository;
        }

        public async Task<List<MutualFundHoldings>> GetMFHoldings()
        {
            return await _mutualFundRepository.GetMFHoldings();
        }

        public async Task<(bool success, string message)> ProcessGrowwReportAsync(IFormFile file)
        {
            try
            {
                var filePath = Path.GetTempFileName();
                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var dataTable = ReadGrowwReport(filePath);

                var stockResult = ProcessGrowwReportData(dataTable);

                await _mutualFundRepository.SaveMutualFundDataAsync(stockResult);

                return (true, "File Data Added");
            }
            catch(Exception ex)
            {
                throw new Exception($"Error processing file: {ex.Message}");
            }
        }


        #region MF
        // Updated method to process and structure the data
        private object ProcessGrowwReportData(DataTable table)
        {
            var report = new
            {
                PersonalDetails = new
                {
                    Name = table.Rows[3]["Column1"]?.ToString(),
                    MobileNumber = table.Rows[4]["Column1"]?.ToString(),
                    PAN = table.Rows[5]["Column1"]?.ToString()
                },
                HoldingSummary = new
                {
                    TotalInvestments = table.Rows[13]["Column0"]?.ToString(),
                    CurrentPortfolioValue = table.Rows[13]["Column1"]?.ToString(),
                    ProfitLoss = table.Rows[13]["Column2"]?.ToString(),
                    ProfitLossPercentage = table.Rows[13]["Column3"]?.ToString(),
                    XIRR = table.Rows[13]["Column4"]?.ToString()
                },
                HoldingsDate = table.Rows[18]["Column0"]?.ToString(),
                Holdings = GetHoldings(table)
            };

            return report;
        }

        //Extract holdings details from the table
        private List<object> GetHoldings(DataTable table)
        {
            var holdings = new List<object>();

            // Adjust the starting index (19) based on where holdings start
            for(int i = 19; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];

                var holding = new Dictionary<string, object>();

                // Add non-empty fields to the dictionary
                if(!string.IsNullOrWhiteSpace(row["Column0"]?.ToString()))
                    holding["SchemeName"] = row["Column0"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column1"]?.ToString()))
                    holding["AMC"] = row["Column1"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column2"]?.ToString()))
                    holding["Category"] = row["Column2"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column3"]?.ToString()))
                    holding["SubCategory"] = row["Column3"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column4"]?.ToString()))
                    holding["FolioNo"] = row["Column4"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column5"]?.ToString()))
                    holding["Source"] = row["Column5"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column6"]?.ToString()))
                    holding["Units"] = row["Column6"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column7"]?.ToString()))
                    holding["InvestedValue"] = row["Column7"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column8"]?.ToString()))
                    holding["CurrentValue"] = row["Column8"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column9"]?.ToString()))
                    holding["Returns"] = row["Column9"]?.ToString()!;
                if(!string.IsNullOrWhiteSpace(row["Column10"]?.ToString()))
                    holding["XIRR"] = row["Column10"]?.ToString()!;

                holdings.Add(holding);
            }

            return holdings;
        }


        #endregion

        #region Read Excel file
        private DataTable ReadGrowwReport(string filePath)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
                using var reader = ExcelReaderFactory.CreateReader(stream);

                var result = reader.AsDataSet();
                if(result.Tables.Count == 0)
                    throw new Exception("The uploaded file does not contain any data.");

                return result.Tables[0];
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to read the Excel file: {ex.Message}");
            }
        }
        #endregion
    }
}
