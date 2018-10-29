using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebHotel.Data;

namespace WebHotel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // create the WebHost object
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                // get the service providers
                var services = scope.ServiceProvider;
                try
                {
                    var serviceProvider = services.GetRequiredService<IServiceProvider>();
                    // get the Config object for the appsettings.json file 
                    var configuration = services.GetRequiredService<IConfiguration>();
                    // pass the service proiders and the config object to CreateRoles()
                    SeedRoles.CreateRoles(serviceProvider, configuration).Wait();

                }
                catch (Exception exception)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "An error occurred while creating roles");
                }
            }

            // start the web app
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
