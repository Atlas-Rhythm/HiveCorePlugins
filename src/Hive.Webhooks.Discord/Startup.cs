using Hive.Plugins;
using Hive.Webhooks.Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Hive.Webhooks
{
    [PluginStartup]
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
            => Configuration = config;

        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.Configure<DiscordHookSettings>(Configuration);
            _ = services.AddSingleton(sp => sp.GetRequiredService<IOptions<DiscordHookSettings>>().Value);
        }
    }
}
