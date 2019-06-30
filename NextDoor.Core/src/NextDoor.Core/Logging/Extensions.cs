using Microsoft.AspNetCore.Hosting;
using NextDoor.Core.Common;
using NextDoor.Core.Types;
using Serilog;
using Serilog.Events;
using System;

namespace NextDoor.Core.Logging
{
    public static class Extensions
    {
        public static IWebHostBuilder UseSeriLogging(this IWebHostBuilder webHostBuilder, string applicationName = null)
            => webHostBuilder.UseSerilog((context, loggerConfiguration) =>
            {
                // create a new instance of "AppOptions" and bind its properties value with JSON "mvc" in appsetting.json
                var appOptions = context.Configuration.GetOptions<MvcOptions>(ConfigOptions.mvcSectionName);
                // create a new instance of "SeqOptions" and bind its properties value with JSON "seq" in appsetting.json
                var seqOptions = context.Configuration.GetOptions<SeqOptions>(ConfigOptions.seqSectionName);
                // create a new instance of "SerilogOptions" and bind its properties value with JSON "serilog" in appsetting.json
                var serilogOptions = context.Configuration.GetOptions<SerilogOptions>(ConfigOptions.serilogSectionName);
                if (!Enum.TryParse<LogEventLevel>(serilogOptions.Level, true, out var level))
                {
                    level = LogEventLevel.Information;
                }

                applicationName = string.IsNullOrWhiteSpace(applicationName) ? appOptions.Name : applicationName;
                loggerConfiguration.Enrich.FromLogContext()
                    .MinimumLevel.Is(level)
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                    .Enrich.WithProperty("ApplicationName", applicationName);
                Configure(loggerConfiguration, level, seqOptions, serilogOptions);
            });

        private static void Configure(LoggerConfiguration loggerConfiguration, LogEventLevel level, SeqOptions seqOptions, SerilogOptions serilogOptions)
        {
            // Enable writing log events to the Seq(https://datalust.co/seq) structured log server.
            if (seqOptions.Enabled)
            {
                loggerConfiguration.WriteTo.Seq(seqOptions.Url, apiKey: seqOptions.ApiKey);
            }

            // Enable writeing log events to the Windows Console via standard output.
            if (serilogOptions.ConsoleEnabled)
            {
                loggerConfiguration.WriteTo.Console();
            }

            // Enable writeing log events to one or more text files.
            if (serilogOptions.FileEnabled)
            {
                loggerConfiguration.WriteTo.File("log_.txt", rollingInterval: RollingInterval.Hour, retainedFileCountLimit: null);
            }
        }
    }
}