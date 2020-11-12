using Serilog;
using Serilog.Sinks.Loki;
using System;

namespace Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var credentials = new NoAuthCredentials("http://localhost:3100"); // Address to local or remote Loki server

            Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .Enrich.FromLogContext()
                    .WriteTo.LokiHttp(credentials)
                    .CreateLogger();

            var position = new { Latitude = 25, Longitude = 134 };
            var elapsedMs = 34;
            Log.Information("Message processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

            Log.CloseAndFlush();
        }
    }
}
