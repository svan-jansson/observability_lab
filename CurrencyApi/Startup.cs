using CurrencyApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using System;

namespace CurrencyApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CurrencyApi", Version = "v1" });
            });
            services.AddHttpClient<IHttpService, HttpService>();

            var serviceName = nameof(CurrencyApi);
            services.AddOpenTelemetryTracing((builder)
                => builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName))
                        .AddSource(serviceName)
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .SetSampler(new AlwaysOnSampler())
                        .AddJaegerExporter(options =>
                        {
                            options.AgentHost = "localhost";
                            options.AgentPort = 6831;
                            options.MaxPayloadSizeInBytes = 65000;
                        }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        private static MetricPusher _metricsPusher;

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CurrencyApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            if (_metricsPusher == default)
            {
                _metricsPusher = new MetricPusher(
                    endpoint: "http://localhost:9091/metrics",
                    job: nameof(CurrencyApi),
                    additionalLabels: new[] { new Tuple<string, string>("domain", "currency") }
                    );
                _metricsPusher.Start();
            }
        }
    }
}
