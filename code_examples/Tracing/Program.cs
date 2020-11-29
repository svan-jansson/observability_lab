using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tracing
{
    class Program
    {
        static void Main(string[] args)
        {
            # region Create Diagnostics Source

            const string ServiceName = "Examples.Tracing";
            var activitySource = new ActivitySource(ServiceName);

            # endregion

            # region Create Tracer Provider

            using var openTelemetry = Sdk.CreateTracerProviderBuilder()
                    .AddSource(ServiceName)
                    .SetResourceBuilder(ResourceBuilder.CreSateDefault().AddService(ServiceName))
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

            using (var activity = activitySource.StartActivity("MyTrace"))
            {
                activity?.SetTag("some.tag", "value");

                using (activitySource.StartActivity("ChildTrace"))
                {
                    Task.Delay(1_000).Wait();
                }

                using (activitySource.StartActivity("AnotherChildTrace"))
                {
                    Task.Delay(1_000).Wait();
                }
            }

            # endregion
        }
    }
}
