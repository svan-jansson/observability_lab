using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Loki;
using Serilog.Sinks.Loki.Labels;
using System;
using System.Collections.Generic;

namespace CurrencyApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Send Logs to Loki using Serilog

            /*
                Required NuGet Packages

                - Serilog.AspNetCore
                - Serilog.Sinks.Loki            
            */

            var lokiCredentials = new NoAuthCredentials("http://localhost:3100");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .Enrich.FromLogContext()
                .WriteTo.LokiHttp(lokiCredentials, new LogLabelProvider())
                .CreateLogger();

            #endregion

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
                    new LokiLabel("app", nameof(CurrencyApi)),
                    new LokiLabel("domain", "currency")
                };
            }

        }
    }
}
