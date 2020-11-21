using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CurrencyApi.Services
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;

        const string ConversionServiceUrl = "https://localhost:7001/CurrencyConversion/";
        const string CurrencyLookupServiceUrl = "https://localhost:6001/CurrencyLookup/";

        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetConversionRate(string fromCurrency, string toCurrency)
        {
            var uri = new Uri(ConversionServiceUrl + fromCurrency + "?to=" + toCurrency);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri
            };
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return Convert.ToDecimal(await response.Content.ReadAsStringAsync());
        }

        public async Task<CurrencyLookupResultDto> LookupCurrency(string currencyCode)
        {
            var uri = new Uri(CurrencyLookupServiceUrl + currencyCode);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uri
            };
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<CurrencyLookupResultDto>(await response.Content.ReadAsStringAsync());
        }
    }
}
