namespace InvestmentPortfolio.Framework.Helper
{
    public static class ConversionHelper
    {
        public static decimal ConvertToDecimal(object value) =>
            decimal.TryParse(Convert.ToString(value), out var result) ? result : 0;

        public static int ConvertToInt(object value) =>
            int.TryParse(Convert.ToString(value), out var result) ? result : 0;

        public static DateTime? ParseNullableDate(string? dateStr) =>
            DateTime.TryParse(dateStr, out var date) ? date : (DateTime?)null;
    }


}
