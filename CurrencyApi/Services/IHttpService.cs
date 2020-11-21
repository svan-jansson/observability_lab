using System.Threading.Tasks;

namespace CurrencyApi.Services
{
    public interface IHttpService
    {
        Task<CurrencyLookupResultDto> LookupCurrency(string currencyCode);
        Task<decimal> GetConversionRate(string fromCurrency, string toCurrency);
    }
}
