using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NextDoor.Services.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// configuring UseKestrel() which uses out-of-process
        /// <AspNetCoreHostingModel>OutOfProcess</AspNetCoreHostingModel>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://localhost:{config.GetValue<int>("host:port")}")
                //.UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>();

            return webHost;
        }

    }
}
