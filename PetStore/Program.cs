using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PetStore
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
                    webBuilder
                        .UseKestrel()
                        // enter your IP address from your local network
                        .UseUrls("http://localhost:55698" /*, "http://192.168.x.x:55698/swagger/index.html"*/)
                        .UseIISIntegration()
                        .UseStartup<Startup>();
                });
    }
}