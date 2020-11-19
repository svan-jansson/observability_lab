using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;

namespace CurrencyLookupService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var lokiCredentials = new NoAuthCredentials("http://localhost:3100");

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.LokiHttp(lokiCredentials, new LogLabelProvider())
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private class LogLabelProvider : ILogLabelProvider
        {
            public IList<LokiLabel> GetLabels()
            {
                return new List<LokiLabel>
                {
                    new LokiLabel("app", nameof(CurrencyLookupService)),
                    new LokiLabel("domain", "currency")
                };
            }

        }
    }
}
