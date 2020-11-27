using CurrencyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CurrencyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyConverterController : ControllerBase
    {

        private readonly ILogger<CurrencyConverterController> _logger;
        private readonly IHttpService _httpService;

        public CurrencyConverterController(ILogger<CurrencyConverterController> logger, IHttpService httpService)
        {
            _logger = logger;
            _httpService = httpService;
        }

        [HttpPost("{from}")]
        public async Task<IActionResult> PostAsync(string from, [FromQuery] decimal value, [FromBody] string[] to)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                return BadRequest();
            }

            if (to == null || to.Length == 0)
            {
                return BadRequest();
            }

            try
            {
                await _httpService.LookupCurrency(from);
            }
            catch
            {
                var traceId = Activity.Current?.TraceId.ToString() ?? HttpContext?.TraceIdentifier;
                _logger.LogError($"Lookup currency {from} failed. traceID={traceId}.");
                throw;
            }

            var response = new List<CurrencyApiResult>();

            foreach (var toCurrency in to)
            {
                try
                {
                    var lookupResponse = await _httpService.LookupCurrency(toCurrency);
                    var conversionRate = await _httpService.GetConversionRate(from, toCurrency);

                    response.Add(new CurrencyApiResult
                    {
                        CurrencyCode = toCurrency,
                        CurrencyName = lookupResponse.CurrencyName,
                        ConversionRate = conversionRate,
                        ConvertedValue = Math.Round(value / conversionRate, 3)
                    });
                }
                catch
                {
                    var traceId = Activity.Current?.TraceId.ToString() ?? HttpContext?.TraceIdentifier;
                    _logger.LogWarning($"Excluding currency {toCurrency} from results. traceID={traceId}.");
                }
            }

            return Ok(response);
        }
    }
}
