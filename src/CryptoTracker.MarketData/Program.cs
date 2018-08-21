using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace CryptoTracker.MarketData
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }


        private static IWebHost BuildWebHost(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .Enrich.FromLogContext()
                .WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();

            return WebHost.CreateDefaultBuilder(args)
                //.ConfigureLogging(x => x.SetMinimumLevel(LogLevel.None))
                .UseStartup<Startup>()
                .UseKestrel()
                .UseSerilog(logger)
                .Build();
        }
    }
}
