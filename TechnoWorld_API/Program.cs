using TechnoWorld_API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace BNS_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ThreadPool.SetMaxThreads(200, 8);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()

                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "{Timestamp:dd.MM.yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
                .CreateBootstrapLogger();

            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: "{Timestamp:dd.MM.yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}"))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}
