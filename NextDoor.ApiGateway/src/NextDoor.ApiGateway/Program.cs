using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NextDoor.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: false)
            //    .Build();

            //var webHost = WebHost.CreateDefaultBuilder(args)
            //    .UseUrls($"http://localhost:{config.GetValue<int>("host:port")}")
            //    //.UseKestrel()
            //    .UseIISIntegration()
            //    .UseStartup<Startup>();

            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://localhost:5200")
                .UseStartup<Startup>()
                //.UseSeriLogging()
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false);

            return webHost;
        }
    }
}
