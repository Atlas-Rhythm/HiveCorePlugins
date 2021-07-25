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
                .AddSingleton<IClientPolicyStore, MemoryCacheClientPolicyStore>()
                .AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>()
                .AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>()
                .AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        public static void Configure(IApplicationBuilder app)
        {
            _ = app.UseClientRateLimiting()
                .UseIpRateLimiting();
        }
    }
}
