using OpenTelemetry;
using OpenTelemetry.Exporter.Prometheus;
using OpenTelemetry.Metrics;
using OpenTelemetry.Metrics.Export;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Metrics
{
    class Program
    {
        static void Main(string[] args)
        {
            var port = 9999;
            var promOptions = new PrometheusExporterOptions() { Url = $"http://localhost:{port}/metrics/" };
            var promExporter = new PrometheusExporter(promOptions);
            var metricsHttpServer = new PrometheusExporterMetricsHttpServer(promExporter);
            metricsHttpServer.Start();

            var processor = new UngroupedBatcher();

            MeterProvider.SetDefault(Sdk.CreateMeterProviderBuilder()
                .SetProcessor(processor)
                .SetExporter(promExporter)
                .SetPushInterval(TimeSpan.FromSeconds(1))
                .Build());

            var meterProvider = MeterProvider.Default;

            var meter = meterProvider.GetMeter("MyMeter");

            var testCounter = meter.CreateInt64Counter("MyCounter");
            var defaultContext = default(SpanContext);

            for (var i = 0; i < 1000; i++)
            {
                testCounter.Add(defaultContext, 1, meter.GetLabelSet(new[] { KeyValuePair.Create("my", "test") }));
                Task.Delay(1_000).Wait();
            }

            metricsHttpServer.Stop();
        }
    }
}
