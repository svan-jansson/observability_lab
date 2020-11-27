using CurrencyConversionService.Caches;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CurrencyConversionService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyConversionController : ControllerBase
    {
        private readonly ILogger<CurrencyConversionController> _logger;

        public CurrencyConversionController(ILogger<CurrencyConversionController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{from}")]
        public decimal Get(string from, [FromQuery] string to)
        {
            var fromRate = ConversionRateCache.Get(from);
            var toRate = ConversionRateCache.Get(to);

            // Simulate slow service
            Task.Delay(rng.Next(10, 200)).Wait();

            return toRate / fromRate;
        }

        private readonly Random rng = new Random();
    }
}
