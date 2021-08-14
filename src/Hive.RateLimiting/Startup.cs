using System;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Hive.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.RateLimiting
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddMemoryCache()
                .Configure<ClientRateLimitOptions>(Configuration.GetSection("ClientRateLimiting"))
                .Configure<ClientRateLimitPolicies>(Configuration.GetSection("ClientRateLimitPolicies"))
                .Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"))
                .Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"))
                .AddInMemoryRateLimiting()
                .AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        public static async Task PreConfigureAsync(IServiceProvider provider)
        {
            await provider.GetRequiredService<IIpPolicyStore>().SeedAsync().ConfigureAwait(false);

            await provider.GetRequiredService<IClientPolicyStore>().SeedAsync().ConfigureAwait(false);
        }

        public static void Configure(IApplicationBuilder app)
        {
            _ = app.UseClientRateLimiting()
                .UseIpRateLimiting();
        }
    }
}
