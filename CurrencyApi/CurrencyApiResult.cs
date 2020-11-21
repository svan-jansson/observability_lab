namespace CurrencyApi
{
    public record CurrencyApiResult
    {
        public string CurrencyCode { get; init; }
        public string CurrencyName { get; init; }
        public decimal ConversionRate { get; init; }
        public decimal ConvertedValue { get; init; }
    }
}
