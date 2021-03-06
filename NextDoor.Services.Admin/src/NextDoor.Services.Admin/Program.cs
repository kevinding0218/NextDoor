﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace NextDoor.Services.Admin
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
                .UseUrls($"http://localhost:5203")
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false);

            return webHost;
        }
    }
}
