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
        /*
            NuGet Packages
                - OpenTelemetry
                - OpenTelemetry.Exporter.Prometheus
        */
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

            # region Make some Measurements
            var labels = new[] { KeyValuePair.Create("my", "label") };

            var meterProvider = MeterProvider.Default;
            var meter = meterProvider.GetMeter("demo_meter");

            var counterMeasurement = meter.CreateInt64Counter("iteration_counter");
            var valueMeasurement = meter.CreateInt64Measure("iteration_measure");
            var observerMeasurement = meter.CreateInt64Observer("memory_observer", (o) => o.Observe(GC.GetTotalMemory(true), labels));

            # endregion

            # region Record Some Metrics!

            var defaultContext = default(SpanContext);
            for (var i = 0; i < 1000; i++)
            {
                counterMeasurement.Add(defaultContext, 1, labels);
                valueMeasurement.Record(defaultContext, i, labels);
                Task.Delay(1_000).Wait();
            }

            # endregion
        }
    }
}
