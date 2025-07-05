using System.Text.Json.Serialization;

namespace InvestmentPortfolio.Models.MFPerformance
{
    public class Scheme
    {
        [JsonPropertyName("schemeCode")]
        public int SchemeCode { get; set; }

        [JsonPropertyName("SchemeName")]
        public string? SchemeName { get; set; }

        [JsonPropertyName("SchemeReturns")]
        public List<SchemeReturn> SchemeReturns { get; set; }
    }

    public class SchemeReturn
    {
        [JsonPropertyName("Date")]
        public string? Date { get; set; }

        [JsonPropertyName("NAV")]
        public decimal NAV { get; set; }

        [JsonPropertyName("CurrentValue")]
        public decimal CurrentValue { get; set; }

        [JsonPropertyName("ReturnPercentage")]
        public decimal ReturnPercentage { get; set; }

        [JsonPropertyName("DayReturn")]
        public decimal DayReturn { get; set; }
    }
}
