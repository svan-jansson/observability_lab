using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CurrencyLookupService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyLookupController : ControllerBase
    {
        private static readonly Dictionary<string, string> ValidCurrencies = new Dictionary<string, string>
        {
            { "SEK", "Swedish krona" },
            { "EUR", "Euro" },
            { "USD", "United States dollar" },
            { "GBP", "British pound" },
            { "DKK", "Danish krone" },
            { "NOK", "Norwegian krone" },
            { "CNY", "Chinese yuan" },
            { "JPY", "Japanese yen" },
            { "EGP", "Egyptian pound" },
            { "BRL", "Brazilian real" }
        };

        private readonly ILogger<CurrencyLookupController> _logger;

        public CurrencyLookupController(ILogger<CurrencyLookupController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{currencyCode}")]
        public IActionResult Get(string currencyCode)
        {
            if (string.IsNullOrWhiteSpace(currencyCode))
            {
                return BadRequest($"{nameof(currencyCode)} cannot be empty");
            }

            currencyCode = currencyCode.ToUpperInvariant().TrimEnd();

            if (!ValidCurrencies.ContainsKey(currencyCode))
            {
                return NotFound(currencyCode);
            }

            return Ok(new CurrencyLookupResult
            {
                CurrencyCode = currencyCode,
                CurrencyName = ValidCurrencies[currencyCode]
            });
        }
    }
}
