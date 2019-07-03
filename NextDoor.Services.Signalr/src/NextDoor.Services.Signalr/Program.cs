using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NextDoor.Services.Signalr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://localhost:5210")
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false);

            return webHost;
        }
    }
}
