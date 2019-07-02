using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NextDoor.Services.Operations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// configuring UseKestrel() which uses out-of-process
        ///     <AspNetCoreHostingModel>OutOfProcess</AspNetCoreHostingModel>
        /// configuring UseIISIntegration() which uses in-process
        ///     <AspNetCoreHostingModel>InProcess</AspNetCoreHostingModel>
        /// </summary>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false)
            //    .Build();

            //var hostPort = config.GetValue<int>("host:port");
            //var webHost = WebHost.CreateDefaultBuilder(args)
            //    .UseUrls($"http://localhost:{hostPort}")
            //    //.UseKestrel()
            //    .UseIISIntegration()
            //    .UseStartup<Startup>();

            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://localhost:5208")
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false);

            return webHost;
        }
    }
}
