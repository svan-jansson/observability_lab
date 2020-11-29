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
            # region Create a Server for Exporting Metrics

            var port = 9999;
            var promOptions = new PrometheusExporterOptions() { Url = $"http://localhost:{port}/metrics/" };
            var promExporter = new PrometheusExporter(promOptions);
            var metricsHttpServer = new PrometheusExporterMetricsHttpServer(promExporter);
            metricsHttpServer.Start();

            # endregion

            # region Create a Meter Provider

            var processor = new UngroupedBatcher();

            MeterProvider.SetDefault(Sdk.CreateMeterProviderBuilder()
                .SetProcessor(processor)
                .SetExporter(promExporter)
                .SetPushInterval(TimeSpan.FromSeconds(1))
                .Build());

            # endregion

            # region Create a Simple Counter Meter

            var meterProvider = MeterProvider.Default;
            var meter = meterProvider.GetMeter("DemoMeter");
            var testCounter = meter.CreateInt64Counter("SimpleCounterMeter");

            # endregion

            # region Record Some Metrics!

            var defaultContext = default(SpanContext);
            for (var i = 0; i < 1000; i++)
            {
                testCounter.Add(defaultContext, 1, meter.GetLabelSet(new[] { KeyValuePair.Create("my", "test") }));
                Task.Delay(1_000).Wait();
            }

            # endregion
        }
    }
}
