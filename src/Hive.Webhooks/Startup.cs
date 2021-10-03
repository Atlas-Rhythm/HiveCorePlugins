using Hive.Plugins;
using Hive.Services.Common;
using Hive.Webhooks.Discord;
using Hive.Webhooks.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            _ = services
                .AddSingleton<IGameVersionsPlugin, HookEmitter>()
                .AddSingleton<IChannelsControllerPlugin, HookEmitter>()
                .AddSingleton<IHookConverter, DiscordHookConverter>();

            _ = services.AddHttpClient();
            _ = services.Configure<WebhookSettings>(Configuration.GetSection(nameof(Webhooks)));
            _ = services.AddSingleton(sp => sp.GetRequiredService<IOptions<WebhookSettings>>().Value);
        }
    }
}
