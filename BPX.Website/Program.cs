using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using System;

namespace BPX.Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //.ConfigureAppConfiguration((context, configuration) =>
            //{
            //    configuration.Sources.Clear();
            //    var bpxEnvironmentName = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
            //    configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            //    configuration.AddJsonFile("appsettings.{bpxEnvironmentName}.json", optional: true, reloadOnChange: true);
            //    configuration.AddCommandLine(args);
            //})
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddNLog(hostingContext.Configuration.GetSection("Logging"));                
            });
    }
}
