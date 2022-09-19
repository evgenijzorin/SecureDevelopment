using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CardStorageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options=>
                    {
                        // set a properties and methods
                        options.Listen(IPAddress.Any, 5001, ListenOptions =>
                        {
                            ListenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
                            ListenOptions.UseHttps(@"C:\devcert.pfx", "12345"); // указать номер защищенного ключа и пароль
                        });
                    })
                    .UseStartup<Startup>();
                })
                    // Конфигурирование логера
                    .ConfigureLogging(logging =>               
                    {                     
                        logging.ClearProviders();                    
                        logging.AddConsole();                
                    }).UseNLog(new NLogAspNetCoreOptions() { RemoveLoggerFactoryFilter = true });
    }
}
