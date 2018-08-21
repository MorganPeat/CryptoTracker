using System.IO;
using System.Threading;
using AutoMapper;
using CryptoTracker.MarketData.DataAccess;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoTracker.MarketData
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Startup
    {
        private readonly CancellationTokenSource _cts;

        public Startup(IHostingEnvironment env)
        {
            _cts = new CancellationTokenSource();

            // Use multiple configuration files instead of one big one.
            // https://www.humankode.com/asp-net-core/asp-net-core-configuration-best-practices-for-keeping-secrets-out-of-source-control
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(Path.Combine("config", "appsettings.json"))
                .AddJsonFile(Path.Combine("config", $"appsettings.{env.EnvironmentName}.json"), optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add support for IOptions<> strongly-typed config
            services.AddOptions();

            services.AddMongoRepositories(Configuration.GetSection("MongoDB").Bind);

            services.AddAutoMapper();

            services.AddMvcCore()
                .AddJsonFormatters()
                .AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SchemaUpdater schemaUpdater)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();


            schemaUpdater.UpdateSchema(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}
