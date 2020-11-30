using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tracing
{
    class Program
    {
        /*
            NuGet Packages
                - OpenTelemetry
                - OpenTelemetry.Exporter.Jaeger
        */
        static void Main(string[] args)
        {
            # region Create Diagnostics Source

            const string ServiceName = "Examples.Tracing";
            var activitySource = new ActivitySource(ServiceName);

            # endregion

            # region Create Tracer Provider

            using var openTelemetry = Sdk.CreateTracerProviderBuilder()
                    .AddSource(ServiceName)
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ServiceName))
                    .SetSampler(new AlwaysOnSampler())
                    // .SetSampler(new TraceIdRatioBasedSampler(0.5))
                    // .SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.5)))
                    .AddJaegerExporter(options =>
                        {
                            options.AgentHost = "localhost";
                            options.AgentPort = 6831;
                            options.MaxPayloadSizeInBytes = 65000;
                        })
                    .Build();

            # endregion

            # region Record Some Spans!

            using (var activity = activitySource.StartActivity("MySpan"))
            {
                activity?.SetTag("some.tag", "value");

                using (var childSpan = activitySource.StartActivity("ChildSpan"))
                {
                    childSpan?.AddEvent(new ActivityEvent("Something interesting happened"));
                    Task.Delay(1_000).Wait();
                }

                using (var childSpan = activitySource.StartActivity("AnotherChildSpan"))
                {
                    childSpan?.AddEvent(new ActivityEvent("Something went wrong :("));
                    childSpan?.SetTag("error", true);
                    Task.Delay(1_000).Wait();
                }
            }

            # endregion
        }
    }
}
