using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConversionService.Caches
{
    public static class ConversionRateCache
    {
        private static readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

        public static void Set(string currencyCode, decimal conversionRate)
            => _cache.Set(currencyCode, conversionRate);

        public static decimal Get(string currencyCode)
            => _cache.Get<decimal>(currencyCode);
    }
}
