using CurrencyConversionService.Caches;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyConversionService.BackgroundServices
{
    public class BankScraperBackgroundService : IHostedService, IDisposable
    {
        private readonly ILogger<BankScraperBackgroundService> _logger;
        private Timer _timer;
        private readonly Random _rng = new Random();

        const string BaseCurrencyCode = "SEK";
        private static readonly Dictionary<string, decimal> CurrencyFlatRates = new Dictionary<string, decimal>
        {
            { "SEK", 1 },
            { "EUR", 10 },
            { "USD", 10 },
            { "GBP", 10 },
            { "DKK", 1 },
            { "NOK", 1 },
            { "CNY", 1 },
            { "JPY", 0.1m },
            { "EGP", 0.5m },
            { "BRL", 1 }
        };

        public BankScraperBackgroundService(ILogger<BankScraperBackgroundService> logger)
        {
            _logger = logger;

            // Populate cache with initial rates
            foreach (var rate in CurrencyFlatRates)
            {
                ConversionRateCache.Set(rate.Key, rate.Value);
            }
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bank scraping background service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {

            // Augument the cached rates
            foreach (var rate in CurrencyFlatRates.Where(p => !p.Key.Equals(BaseCurrencyCode)))
            {
                var currentRate = ConversionRateCache.Get(rate.Key);

                var modifier = _rng.Next(-100, 100) / 1000m;
                var newRate = currentRate + modifier;
                if (newRate <= 0)
                {
                    newRate += Math.Abs(modifier);
                }
                newRate = Math.Round(newRate, 3);

                _logger.LogInformation($"Conversion rate for {rate.Key} changed from {currentRate} to {newRate}.");
                ConversionRateCache.Set(rate.Key, newRate);
            }

            _logger.LogInformation("Bank scraping background service updated conversion rates.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bank scraping background service stopped.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
