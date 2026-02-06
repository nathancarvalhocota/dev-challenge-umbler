using Desafio.Umbler.Repositories;
using Desafio.Umbler.Services;
using Desafio.Umbler.Services.Clients;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Desafio.Umbler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddScoped<IDomainRepository, DomainRepository>();
                services.AddScoped<IDomainService, DomainService>();
                services.AddScoped<IWhoisClient, WhoisClientAdapter>();
                services.AddScoped<IDnsLookupClient, DnsLookupClientAdapter>();
            })
            .ConfigureAppConfiguration((context, config) =>
                config
                    .SetBasePath(context.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", false, true)
                    .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
