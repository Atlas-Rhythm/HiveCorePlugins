using DryIoc;
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
            _ = services.AddHttpClient();
            _ = services.Configure<WebhookSettings>(Configuration);
            _ = services.AddSingleton(sp => sp.GetRequiredService<IOptions<WebhookSettings>>().Value);
            _ = services.AddSingleton(sp => sp.GetRequiredService<IOptions<WebhookSettings>>().Value.Discord);
        }

        public void ConfigureContainer(Container container)
        {
            container.RegisterMany<WebhookEmitter>(Reuse.Singleton);
            container.Register<IHiveWebhook, DefaultHiveWebhook>(Reuse.Singleton);
            container.Register<IHiveWebhook, DiscordHiveWebhook>(Reuse.Singleton);
        }
    }
}
